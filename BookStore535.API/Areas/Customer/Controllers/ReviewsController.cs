using BookStore535.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore535.API.Areas.Customer.Controllers;

[Area(CD.CUSTOMER_AREA)]
[Route("api/[area]/Books/{bookId:int}/Reviews")]
[ApiController]
public sealed class ReviewsController(ApplicationDbContext context, UserManager<CustomerAccount> users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetForBook(int bookId, CancellationToken cancellationToken)
    {
        var exists = await context.Books.AnyAsync(book => book.Id == bookId && book.IsActive, cancellationToken);
        if (!exists) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        var reviews = await context.BookReviews.AsNoTracking()
            .Where(review => review.BookId == bookId && review.IsApproved)
            .Include(review => review.CustomerAccount)
            .OrderByDescending(review => review.CreatedUtc)
            .ToListAsync(cancellationToken);
        var response = reviews.Select(review => new BookReviewResponse
        {
            Id = review.Id,
            BookId = review.BookId,
            Rating = review.Rating,
            Comment = review.Comment,
            CustomerName = review.CustomerAccount.Name,
            CreatedUtc = review.CreatedUtc
        }).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<BookReviewResponse>>(response, "Reviews returned successfully."));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(int bookId, BookReviewRequest request, CancellationToken cancellationToken)
    {
        var customerId = int.Parse(users.GetUserId(User)!);
        var bookExists = await context.Books.AnyAsync(book => book.Id == bookId && book.IsActive, cancellationToken);
        if (!bookExists) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        var purchased = await context.BookOrders.AnyAsync(order =>
            order.CustomerAccountId == customerId
            && order.Status == BookOrderStatuses.Delivered
            && order.Items.Any(item => item.BookId == bookId), cancellationToken);
        if (!purchased)
            return Forbid();
        if (await context.BookReviews.AnyAsync(review => review.BookId == bookId && review.CustomerAccountId == customerId, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("You have already reviewed this book."));

        var review = new BookReview
        {
            BookId = bookId,
            CustomerAccountId = customerId,
            Rating = request.Rating,
            Comment = request.Comment?.Trim(),
            CreatedUtc = DateTime.UtcNow
        };
        context.BookReviews.Add(review);
        await context.SaveChangesAsync(cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponseFactory.Success(new BookReviewResponse
        {
            Id = review.Id,
            BookId = review.BookId,
            Rating = review.Rating,
            Comment = review.Comment,
            CustomerName = User.Identity?.Name ?? string.Empty,
            CreatedUtc = review.CreatedUtc
        }, "Review created successfully."));
    }
}
