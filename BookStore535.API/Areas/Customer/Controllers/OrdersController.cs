using BookStore535.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore535.API.Areas.Customer.Controllers;

[Area(CD.CUSTOMER_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize]
public sealed class OrdersController(
    ApplicationDbContext context,
    UserManager<CustomerAccount> users,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var orders = await context.BookOrders.AsNoTracking()
            .Where(order => order.CustomerAccountId == customerId)
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedUtc)
            .ToListAsync(cancellationToken);
        return Ok(ApiResponseFactory.Success<IReadOnlyList<OrderResponse>>(
            orders.Select(ToResponse).ToList(), "Orders returned successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var order = await context.BookOrders.AsNoTracking().Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.Id == id && item.CustomerAccountId == customerId, cancellationToken);
        if (order is null) return NotFound(ApiResponseFactory.Failure<object>("Order not found."));
        return Ok(ApiResponseFactory.Success(ToResponse(order), "Order returned successfully."));
    }

    [HttpPost("Checkout")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Checkout(CheckoutRequest request, CancellationToken cancellationToken)
    {
        if (!string.Equals(request.PaymentMethod, "CashOnDelivery", StringComparison.OrdinalIgnoreCase))
            return BadRequest(ApiResponseFactory.Failure<object>("Only CashOnDelivery is enabled in this backend."));

        var customerId = GetCustomerId();
        var cart = await context.ShoppingCarts.Include(item => item.Items).ThenInclude(item => item.Book)
            .SingleOrDefaultAsync(item => item.CustomerAccountId == customerId, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
            return BadRequest(ApiResponseFactory.Failure<object>("Your cart is empty."));

        foreach (var item in cart.Items)
        {
            if (!item.Book.IsActive) return Conflict(ApiResponseFactory.Failure<object>($"'{item.Book.Title}' is no longer available."));
            if (item.Quantity > item.Book.StockQuantity)
                return Conflict(ApiResponseFactory.Failure<object>($"Only {item.Book.StockQuantity} copies of '{item.Book.Title}' are available."));
        }

        var subtotal = decimal.Round(cart.Items.Sum(item => item.Book.Price * item.Quantity), 2);
        DiscountCode? discountCode = null;
        decimal discountAmount = 0;
        if (!string.IsNullOrWhiteSpace(request.DiscountCode))
        {
            var normalizedCode = request.DiscountCode.Trim().ToUpperInvariant();
            discountCode = await context.DiscountCodes.SingleOrDefaultAsync(item => item.Code == normalizedCode, cancellationToken);
            if (discountCode is null || !CartController.IsDiscountActive(discountCode))
                return BadRequest(ApiResponseFactory.Failure<object>("Discount code is invalid, expired, or used up."));
            if (subtotal < discountCode.MinimumOrderAmount)
                return BadRequest(ApiResponseFactory.Failure<object>($"The minimum order amount for this code is {discountCode.MinimumOrderAmount:C}."));
            discountAmount = decimal.Round(subtotal * discountCode.PercentOff / 100m, 2);
        }

        var shippingAmount = Math.Max(0, configuration.GetValue<decimal>("Checkout:ShippingAmount"));
        var taxRatePercent = Math.Max(0, configuration.GetValue<decimal>("Checkout:TaxRatePercent"));
        var taxAmount = decimal.Round((subtotal - discountAmount) * taxRatePercent / 100m, 2);
        var order = new BookOrder
        {
            OrderNumber = $"BS-{Guid.NewGuid():N}"[..15].ToUpperInvariant(),
            CustomerAccountId = customerId,
            Status = BookOrderStatuses.Pending,
            PaymentMethod = "CashOnDelivery",
            RecipientName = request.RecipientName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            AddressLine = request.AddressLine.Trim(),
            City = request.City.Trim(),
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingAmount = shippingAmount,
            TaxAmount = taxAmount,
            TotalAmount = subtotal - discountAmount + shippingAmount + taxAmount,
            DiscountCode = discountCode?.Code,
            CreatedUtc = DateTime.UtcNow,
            Items = cart.Items.Select(item => new BookOrderItem
            {
                BookId = item.BookId,
                TitleSnapshot = item.Book.Title,
                BookCodeSnapshot = item.Book.BookCode,
                Quantity = item.Quantity,
                UnitPrice = item.Book.Price,
                LineTotal = decimal.Round(item.Book.Price * item.Quantity, 2)
            }).ToList()
        };

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        foreach (var item in cart.Items) item.Book.StockQuantity -= item.Quantity;
        if (discountCode is not null) discountCode.UsesCount++;
        context.BookOrders.Add(order);
        context.CartItems.RemoveRange(cart.Items);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Conflict(ApiResponseFactory.Failure<object>("Inventory changed while placing the order. Refresh the cart and try again."));
        }

        return StatusCode(StatusCodes.Status201Created,
            ApiResponseFactory.Success(ToResponse(order), "Order placed successfully."));
    }

    [HttpPost("{id:int}/Cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var order = await context.BookOrders.Include(item => item.Items).ThenInclude(item => item.Book)
            .SingleOrDefaultAsync(item => item.Id == id && item.CustomerAccountId == customerId, cancellationToken);
        if (order is null) return NotFound(ApiResponseFactory.Failure<object>("Order not found."));
        if (order.Status != BookOrderStatuses.Pending)
            return Conflict(ApiResponseFactory.Failure<object>("Only pending orders can be cancelled."));

        foreach (var item in order.Items) item.Book.StockQuantity += item.Quantity;
        order.Status = BookOrderStatuses.Cancelled;
        await context.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponseFactory.Success(ToResponse(order), "Order cancelled successfully."));
    }

    private int GetCustomerId() => int.Parse(users.GetUserId(User)!);

    private static OrderResponse ToResponse(BookOrder order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        Status = order.Status,
        PaymentMethod = order.PaymentMethod,
        RecipientName = order.RecipientName,
        PhoneNumber = order.PhoneNumber,
        AddressLine = order.AddressLine,
        City = order.City,
        Subtotal = order.Subtotal,
        DiscountAmount = order.DiscountAmount,
        ShippingAmount = order.ShippingAmount,
        TaxAmount = order.TaxAmount,
        TotalAmount = order.TotalAmount,
        DiscountCode = order.DiscountCode,
        CreatedUtc = order.CreatedUtc,
        Items = order.Items.Select(item => new OrderItemResponse
        {
            BookId = item.BookId,
            Title = item.TitleSnapshot,
            BookCode = item.BookCodeSnapshot,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal
        }).ToList()
    };
}
