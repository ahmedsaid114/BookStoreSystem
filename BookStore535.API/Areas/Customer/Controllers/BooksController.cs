using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Customer.Controllers;

[Area(CD.CUSTOMER_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
public sealed class BooksController(IRepository<Book> books) : ControllerBase
{
    private static readonly string[] Includes = ["Category", "Author", "Publisher", "Reviews"];

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<BookResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int? publisherId,
        [FromQuery] int? authorId,
        [FromQuery] int? year,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? featured,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string sortBy = "title",
        [FromQuery] bool descending = false,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        Expression<Func<Book, bool>> filter = book => book.IsActive && book.Category.IsActive
            && (!categoryId.HasValue || book.CategoryId == categoryId.Value)
            && (!publisherId.HasValue || book.PublisherId == publisherId.Value)
            && (!authorId.HasValue || book.AuthorId == authorId.Value)
            && (!year.HasValue || book.PublishedYear == year.Value)
            && (!minPrice.HasValue || book.Price >= minPrice.Value)
            && (!maxPrice.HasValue || book.Price <= maxPrice.Value)
            && (!featured.HasValue || !featured.Value || book.IsFeatured)
            && (search == null || book.Title.Contains(search) || book.BookCode.Contains(search)
                || (book.Author != null && book.Author.Name.Contains(search)));

        Expression<Func<Book, object>> orderBy = sortBy.ToLowerInvariant() switch
        {
            "price" => book => book.Price,
            "year" => book => book.PublishedYear,
            "rating" => book => book.Reviews.Where(review => review.IsApproved).Select(review => (double?)review.Rating).Average() ?? 0d,
            _ => book => book.Title
        };
        var totalCount = await books.CountAsync(filter, cancellationToken);
        var items = await books.GetPageAsync(filter, (page - 1) * pageSize, pageSize, orderBy, Includes, descending, cancellationToken);
        var result = new PagedResponse<BookResponse>
        {
            Items = items.Select(BookMapper.ToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
        return Ok(ApiResponseFactory.Success(result, "Books returned successfully."));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var book = await books.GetOneAsync(item => item.Id == id && item.IsActive && item.Category.IsActive, Includes, false, cancellationToken);
        if (book is null) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        return Ok(ApiResponseFactory.Success(BookMapper.ToResponse(book), "Book returned successfully."));
    }
}
