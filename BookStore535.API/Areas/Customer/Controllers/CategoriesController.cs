using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Customer.Controllers;

[Area(CD.CUSTOMER_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
public sealed class CategoriesController(IRepository<Category> categories) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await categories.GetAllAsync(category => category.IsActive, ["Books"], false, cancellationToken);
        var response = items.OrderBy(category => category.Name).Select(category => new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            BookCount = category.Books.Count(book => book.IsActive)
        }).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<CategoryResponse>>(response, "Categories returned successfully."));
    }
}
