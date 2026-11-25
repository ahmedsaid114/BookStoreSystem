using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Admin.Controllers;

[Area(CD.ADMIN_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.AdminAccess)]
public sealed class BooksController(
    IRepository<Book> books,
    IRepository<Category> categories,
    IRepository<Author> authors,
    IRepository<Publisher> publishers) : ControllerBase
{
    private static readonly string[] Includes = ["Category", "Author", "Publisher", "Reviews"];

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await books.GetAllAsync(includes: Includes, isTracked: false, cancellationToken: cancellationToken);
        var response = items.OrderBy(book => book.Title).Select(BookMapper.ToResponse).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<BookResponse>>(response, "Books returned successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var book = await books.GetOneAsync(item => item.Id == id, Includes, false, cancellationToken);
        if (book is null) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        return Ok(ApiResponseFactory.Success(BookMapper.ToResponse(book), "Book returned successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookRequest request, CancellationToken cancellationToken)
    {
        var error = await ValidateReferences(request, cancellationToken);
        if (error is not null) return BadRequest(ApiResponseFactory.Failure<object>(error));
        if (await books.AnyAsync(book => book.BookCode == request.BookCode.Trim(), cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("A book with this code already exists."));

        var book = request.Adapt<Book>();
        book.Title = request.Title.Trim();
        book.BookCode = request.BookCode.Trim();
        book.Description = request.Description?.Trim();
        book.CreatedUtc = DateTime.UtcNow;
        await books.AddAsync(book, cancellationToken);
        await books.CommitAsync(cancellationToken);
        var created = await books.GetOneAsync(item => item.Id == book.Id, Includes, false, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = book.Id },
            ApiResponseFactory.Success(BookMapper.ToResponse(created!), "Book created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BookRequest request, CancellationToken cancellationToken)
    {
        var book = await books.GetOneAsync(item => item.Id == id, cancellationToken: cancellationToken);
        if (book is null) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        var error = await ValidateReferences(request, cancellationToken);
        if (error is not null) return BadRequest(ApiResponseFactory.Failure<object>(error));
        if (await books.AnyAsync(item => item.Id != id && item.BookCode == request.BookCode.Trim(), cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("A book with this code already exists."));

        request.Adapt(book);
        book.Title = request.Title.Trim();
        book.BookCode = request.BookCode.Trim();
        book.Description = request.Description?.Trim();
        await books.CommitAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var book = await books.GetOneAsync(item => item.Id == id, cancellationToken: cancellationToken);
        if (book is null) return NotFound(ApiResponseFactory.Failure<object>("Book not found."));
        book.IsActive = false;
        book.StockQuantity = 0;
        await books.CommitAsync(cancellationToken);
        return NoContent();
    }

    private async Task<string?> ValidateReferences(BookRequest request, CancellationToken cancellationToken)
    {
        if (!await categories.AnyAsync(item => item.Id == request.CategoryId, cancellationToken)) return "CategoryId does not exist.";
        if (!await authors.AnyAsync(item => item.Id == request.AuthorId, cancellationToken)) return "AuthorId does not exist.";
        if (!await publishers.AnyAsync(item => item.Id == request.PublisherId, cancellationToken)) return "PublisherId does not exist.";
        if (request.OriginalPrice.HasValue && request.OriginalPrice.Value < request.Price)
            return "OriginalPrice must be greater than or equal to Price.";
        return null;
    }
}
