using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Admin.Controllers;

[Area(CD.ADMIN_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.AdminAccess)]
public sealed class AuthorsController(IRepository<Author> authors, IRepository<Book> books) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await authors.GetAllAsync(includes: ["Books"], isTracked: false, cancellationToken: cancellationToken);
        var response = items.OrderBy(item => item.Name).Select(item => new AuthorResponse
        {
            Id = item.Id, Name = item.Name, Biography = item.Biography, ImageUrl = item.ImageUrl,
            BookCount = item.Books.Count(book => book.IsActive)
        }).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<AuthorResponse>>(response, "Authors returned successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await authors.GetOneAsync(author => author.Id == id, isTracked: false, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Author not found."));
        return Ok(ApiResponseFactory.Success(item.Adapt<AuthorResponse>(), "Author returned successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(AuthorRequest request, CancellationToken cancellationToken)
    {
        var item = request.Adapt<Author>();
        item.Name = request.Name.Trim();
        item.Biography = request.Biography?.Trim();
        await authors.AddAsync(item, cancellationToken);
        await authors.CommitAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponseFactory.Success(item.Adapt<AuthorResponse>(), "Author created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AuthorRequest request, CancellationToken cancellationToken)
    {
        var item = await authors.GetOneAsync(author => author.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Author not found."));
        request.Adapt(item);
        item.Name = request.Name.Trim();
        item.Biography = request.Biography?.Trim();
        await authors.CommitAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await authors.GetOneAsync(author => author.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Author not found."));
        if (await books.AnyAsync(book => book.AuthorId == id, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Author is in use.", ["Move or deactivate the author's books before deleting the author."]));
        authors.Remove(item);
        await authors.CommitAsync(cancellationToken);
        return NoContent();
    }
}
