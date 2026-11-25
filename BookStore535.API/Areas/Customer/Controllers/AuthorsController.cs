using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Customer.Controllers;

[Area(CD.CUSTOMER_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
public sealed class AuthorsController(IRepository<Author> authors) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await authors.GetAllAsync(includes: ["Books"], isTracked: false, cancellationToken: cancellationToken);
        var response = items.OrderBy(author => author.Name).Select(author => new AuthorResponse
        {
            Id = author.Id,
            Name = author.Name,
            Biography = author.Biography,
            ImageUrl = author.ImageUrl,
            BookCount = author.Books.Count(book => book.IsActive)
        }).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<AuthorResponse>>(response, "Authors returned successfully."));
    }
}
