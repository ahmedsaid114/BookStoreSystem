using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Admin.Controllers;

[Area(CD.ADMIN_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.AdminAccess)]
public sealed class CategoriesController(IRepository<Category> categories, IRepository<Book> books) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await categories.GetAllAsync(includes: ["Books"], isTracked: false, cancellationToken: cancellationToken);
        var response = items.OrderBy(item => item.Name).Select(item => new CategoryResponse
        {
            Id = item.Id, Name = item.Name, Description = item.Description, ImageUrl = item.ImageUrl,
            BookCount = item.Books.Count(book => book.IsActive)
        }).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<CategoryResponse>>(response, "Categories returned successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await categories.GetOneAsync(category => category.Id == id, isTracked: false, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Category not found."));
        return Ok(ApiResponseFactory.Success(item.Adapt<CategoryResponse>(), "Category returned successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await categories.AnyAsync(category => category.Name == name, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Category name already exists."));
        var item = request.Adapt<Category>();
        item.Name = name;
        item.Description = request.Description?.Trim();
        await categories.AddAsync(item, cancellationToken);
        await categories.CommitAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponseFactory.Success(item.Adapt<CategoryResponse>(), "Category created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryRequest request, CancellationToken cancellationToken)
    {
        var item = await categories.GetOneAsync(category => category.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Category not found."));
        var name = request.Name.Trim();
        if (await categories.AnyAsync(category => category.Id != id && category.Name == name, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Category name already exists."));
        request.Adapt(item);
        item.Name = name;
        item.Description = request.Description?.Trim();
        await categories.CommitAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await categories.GetOneAsync(category => category.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Category not found."));
        if (await books.AnyAsync(book => book.CategoryId == id, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Category is in use.", ["Move or deactivate its books before deleting it."]));
        categories.Remove(item);
        await categories.CommitAsync(cancellationToken);
        return NoContent();
    }
}
