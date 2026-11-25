using BookStore535.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore535.API.Areas.Admin.Controllers;

[Area(CD.ADMIN_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.AdminAccess)]
public sealed class OrdersController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orders = await context.BookOrders.AsNoTracking().Include(order => order.Items)
            .OrderByDescending(order => order.CreatedUtc).ToListAsync(cancellationToken);
        return Ok(ApiResponseFactory.Success<IReadOnlyList<OrderResponse>>(orders.Select(ToResponse).ToList(), "Orders returned successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await context.BookOrders.AsNoTracking().Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (order is null) return NotFound(ApiResponseFactory.Failure<object>("Order not found."));
        return Ok(ApiResponseFactory.Success(ToResponse(order), "Order returned successfully."));
    }

    [HttpPut("{id:int}/Status")]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatusRequest request, CancellationToken cancellationToken)
    {
        var allowed = new[] { BookOrderStatuses.Pending, BookOrderStatuses.Processing, BookOrderStatuses.Shipped, BookOrderStatuses.Delivered, BookOrderStatuses.Cancelled };
        var next = allowed.FirstOrDefault(status => string.Equals(status, request.Status.Trim(), StringComparison.OrdinalIgnoreCase));
        if (next is null) return BadRequest(ApiResponseFactory.Failure<object>("Status must be Pending, Processing, Shipped, Delivered, or Cancelled."));

        var order = await context.BookOrders.Include(item => item.Items).ThenInclude(item => item.Book)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (order is null) return NotFound(ApiResponseFactory.Failure<object>("Order not found."));
        if (order.Status == BookOrderStatuses.Delivered || order.Status == BookOrderStatuses.Cancelled)
            return Conflict(ApiResponseFactory.Failure<object>("Delivered or cancelled orders cannot be changed."));

        if (next == BookOrderStatuses.Cancelled)
        {
            foreach (var item in order.Items) item.Book.StockQuantity += item.Quantity;
        }
        order.Status = next;
        await context.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponseFactory.Success(ToResponse(order), "Order status updated successfully."));
    }

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
            BookId = item.BookId, Title = item.TitleSnapshot, BookCode = item.BookCodeSnapshot,
            Quantity = item.Quantity, UnitPrice = item.UnitPrice, LineTotal = item.LineTotal
        }).ToList()
    };
}
