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
public sealed class CartController(ApplicationDbContext context, UserManager<CustomerAccount> users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart([FromQuery] string? discountCode, CancellationToken cancellationToken)
    {
        var cart = await GetCartAsync(createIfMissing: false, cancellationToken);
        if (cart is null)
            return Ok(ApiResponseFactory.Success(new CartResponse(), "Cart returned successfully."));
        var response = await BuildResponseAsync(cart, discountCode, cancellationToken);
        if (response.Error is not null) return BadRequest(ApiResponseFactory.Failure<object>(response.Error));
        return Ok(ApiResponseFactory.Success(response.Cart, "Cart returned successfully."));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var book = await context.Books.SingleOrDefaultAsync(item => item.Id == request.BookId && item.IsActive, cancellationToken);
        if (book is null) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        var cart = await GetCartAsync(createIfMissing: true, cancellationToken);
        var existing = cart!.Items.SingleOrDefault(item => item.BookId == request.BookId);
        var newQuantity = (existing?.Quantity ?? 0) + request.Quantity;
        if (newQuantity > book.StockQuantity)
            return Conflict(ApiResponseFactory.Failure<object>("Not enough stock.", [$"Only {book.StockQuantity} copies are available."]));

        if (existing is null) cart.Items.Add(new CartItem { BookId = book.Id, Quantity = request.Quantity });
        else existing.Quantity = newQuantity;
        cart.UpdatedUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        var loaded = await LoadCartAsync(cart.Id, cancellationToken);
        return Ok(ApiResponseFactory.Success((await BuildResponseAsync(loaded!, null, cancellationToken)).Cart, "Book added to cart."));
    }

    [HttpPut("items/{itemId:int}")]
    public async Task<IActionResult> UpdateItem(int itemId, UpdateCartItemRequest request, CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var item = await context.CartItems.Include(cartItem => cartItem.Book)
            .Include(cartItem => cartItem.ShoppingCart)
            .SingleOrDefaultAsync(cartItem => cartItem.Id == itemId && cartItem.ShoppingCart.CustomerAccountId == customerId, cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Cart item not found."));
        if (request.Quantity > item.Book.StockQuantity)
            return Conflict(ApiResponseFactory.Failure<object>("Not enough stock.", [$"Only {item.Book.StockQuantity} copies are available."]));

        item.Quantity = request.Quantity;
        item.ShoppingCart.UpdatedUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        var cart = await LoadCartAsync(item.ShoppingCartId, cancellationToken);
        return Ok(ApiResponseFactory.Success((await BuildResponseAsync(cart!, null, cancellationToken)).Cart, "Cart item updated."));
    }

    [HttpDelete("items/{itemId:int}")]
    public async Task<IActionResult> RemoveItem(int itemId, CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var item = await context.CartItems.Include(cartItem => cartItem.ShoppingCart)
            .SingleOrDefaultAsync(cartItem => cartItem.Id == itemId && cartItem.ShoppingCart.CustomerAccountId == customerId, cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Cart item not found."));
        context.CartItems.Remove(item);
        item.ShoppingCart.UpdatedUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("ApplyDiscount")]
    public async Task<IActionResult> ApplyDiscount(ApplyDiscountRequest request, CancellationToken cancellationToken)
    {
        var cart = await GetCartAsync(createIfMissing: false, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
            return BadRequest(ApiResponseFactory.Failure<object>("Add books to your cart before applying a discount."));
        var response = await BuildResponseAsync(cart, request.Code, cancellationToken);
        if (response.Error is not null) return BadRequest(ApiResponseFactory.Failure<object>(response.Error));
        return Ok(ApiResponseFactory.Success(response.Cart, "Discount code applied to this cart total."));
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        var cart = await GetCartAsync(createIfMissing: false, cancellationToken);
        if (cart is null) return NoContent();
        context.CartItems.RemoveRange(cart.Items);
        cart.UpdatedUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<ShoppingCart?> GetCartAsync(bool createIfMissing, CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var cart = await context.ShoppingCarts.Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.CustomerAccountId == customerId, cancellationToken);
        if (cart is not null || !createIfMissing) return cart;
        cart = new ShoppingCart { CustomerAccountId = customerId };
        context.ShoppingCarts.Add(cart);
        await context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    private Task<ShoppingCart?> LoadCartAsync(int id, CancellationToken cancellationToken) =>
        context.ShoppingCarts.Include(cart => cart.Items).ThenInclude(item => item.Book).ThenInclude(book => book.Author)
            .SingleOrDefaultAsync(cart => cart.Id == id, cancellationToken);

    private async Task<(CartResponse Cart, string? Error)> BuildResponseAsync(ShoppingCart cart, string? discountCode, CancellationToken cancellationToken)
    {
        var items = await LoadCartAsync(cart.Id, cancellationToken) ?? cart;
        var responseItems = items.Items.Select(item => new CartItemResponse
        {
            Id = item.Id,
            BookId = item.BookId,
            Title = item.Book.Title,
            CoverImageUrl = item.Book.CoverImageUrl,
            AuthorName = item.Book.Author?.Name ?? string.Empty,
            BookCode = item.Book.BookCode,
            Quantity = item.Quantity,
            UnitPrice = item.Book.Price,
            LineTotal = decimal.Round(item.Book.Price * item.Quantity, 2)
        }).ToList();
        var subtotal = responseItems.Sum(item => item.LineTotal);
        decimal discount = 0;
        string? appliedCode = null;
        if (!string.IsNullOrWhiteSpace(discountCode))
        {
            var code = discountCode.Trim().ToUpperInvariant();
            var coupon = await context.DiscountCodes.SingleOrDefaultAsync(item => item.Code == code, cancellationToken);
            if (coupon is null || !IsDiscountActive(coupon)) return (new CartResponse(), "Discount code is invalid or expired.");
            if (subtotal < coupon.MinimumOrderAmount) return (new CartResponse(), $"Minimum order amount for this code is {coupon.MinimumOrderAmount:C}.");
            discount = decimal.Round(subtotal * coupon.PercentOff / 100m, 2);
            appliedCode = coupon.Code;
        }

        return (new CartResponse
        {
            Items = responseItems,
            ItemCount = responseItems.Sum(item => item.Quantity),
            Subtotal = subtotal,
            DiscountAmount = discount,
            Total = subtotal - discount,
            AppliedDiscountCode = appliedCode
        }, null);
    }

    private int GetCustomerId() => int.Parse(users.GetUserId(User)!);

    internal static bool IsDiscountActive(DiscountCode code)
    {
        var now = DateTime.UtcNow;
        return code.IsActive
            && (!code.StartsUtc.HasValue || code.StartsUtc <= now)
            && (!code.ExpiresUtc.HasValue || code.ExpiresUtc >= now)
            && (!code.MaxUses.HasValue || code.UsesCount < code.MaxUses.Value);
    }
}
