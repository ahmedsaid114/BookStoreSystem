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
public sealed class WishlistController(ApplicationDbContext context, UserManager<CustomerAccount> users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetWishlist(CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var list = await context.Wishlists
            .Include("Items.Book.Category").Include("Items.Book.Author").Include("Items.Book.Publisher").Include("Items.Book.Reviews")
            .SingleOrDefaultAsync(item => item.CustomerAccountId == customerId, cancellationToken);
        var response = new WishlistResponse
        {
            Items = list?.Items.Select(item => BookMapper.ToResponse(item.Book)).ToList() ?? []
        };
        return Ok(ApiResponseFactory.Success(response, "Wishlist returned successfully."));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddWishlistItemRequest request, CancellationToken cancellationToken)
    {
        var bookExists = await context.Books.AnyAsync(book => book.Id == request.BookId && book.IsActive, cancellationToken);
        if (!bookExists) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        var list = await GetOrCreateWishlistAsync(cancellationToken);
        if (list.Items.Any(item => item.BookId == request.BookId))
            return Ok(ApiResponseFactory.Success<object?>(null, "Book is already in the wishlist."));
        list.Items.Add(new WishlistItem { BookId = request.BookId });
        list.UpdatedUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponseFactory.Success<object?>(null, "Book added to wishlist."));
    }

    [HttpDelete("items/{bookId:int}")]
    public async Task<IActionResult> RemoveItem(int bookId, CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var item = await context.WishlistItems.Include(entry => entry.Wishlist)
            .SingleOrDefaultAsync(entry => entry.BookId == bookId && entry.Wishlist.CustomerAccountId == customerId, cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Wishlist item not found."));
        context.WishlistItems.Remove(item);
        item.Wishlist.UpdatedUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("items/{bookId:int}/move-to-cart")]
    public async Task<IActionResult> MoveToCart(int bookId, CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var wishItem = await context.WishlistItems.Include(item => item.Wishlist)
            .SingleOrDefaultAsync(item => item.BookId == bookId && item.Wishlist.CustomerAccountId == customerId, cancellationToken);
        if (wishItem is null) return NotFound(ApiResponseFactory.Failure<object>("Wishlist item not found."));
        var book = await context.Books.SingleOrDefaultAsync(item => item.Id == bookId && item.IsActive, cancellationToken);
        if (book is null) return NotFound(ApiResponseFactory.Failure<object>("Book is no longer available."));
        if (book.StockQuantity < 1) return Conflict(ApiResponseFactory.Failure<object>("Book is out of stock."));

        var cart = await context.ShoppingCarts.Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.CustomerAccountId == customerId, cancellationToken);
        if (cart is null)
        {
            cart = new ShoppingCart { CustomerAccountId = customerId };
            context.ShoppingCarts.Add(cart);
        }
        var cartItem = cart.Items.SingleOrDefault(item => item.BookId == bookId);
        if (cartItem is null) cart.Items.Add(new CartItem { BookId = bookId, Quantity = 1 });
        else if (cartItem.Quantity >= book.StockQuantity)
            return Conflict(ApiResponseFactory.Failure<object>("Not enough stock for another copy."));
        else cartItem.Quantity++;

        context.WishlistItems.Remove(wishItem);
        cart.UpdatedUtc = DateTime.UtcNow;
        wishItem.Wishlist.UpdatedUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponseFactory.Success<object?>(null, "Book moved to cart."));
    }

    private async Task<Wishlist> GetOrCreateWishlistAsync(CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        var list = await context.Wishlists.Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.CustomerAccountId == customerId, cancellationToken);
        if (list is not null) return list;
        list = new Wishlist { CustomerAccountId = customerId };
        context.Wishlists.Add(list);
        await context.SaveChangesAsync(cancellationToken);
        return list;
    }

    private int GetCustomerId() => int.Parse(users.GetUserId(User)!);
}
