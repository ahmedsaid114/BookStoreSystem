using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Admin.Controllers;

[Area(CD.ADMIN_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.AdminAccess)]
public sealed class PublishersController(IRepository<Publisher> publishers, IRepository<Book> books) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await publishers.GetAllAsync(includes: ["Books"], isTracked: false, cancellationToken: cancellationToken);
        var response = items.OrderBy(item => item.Name).Select(item => new PublisherResponse
        {
            Id = item.Id, Name = item.Name, Description = item.Description, WebsiteUrl = item.WebsiteUrl,
            BookCount = item.Books.Count(book => book.IsActive)
        }).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<PublisherResponse>>(response, "Publishers returned successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await publishers.GetOneAsync(publisher => publisher.Id == id, isTracked: false, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Publisher not found."));
        return Ok(ApiResponseFactory.Success(item.Adapt<PublisherResponse>(), "Publisher returned successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(PublisherRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await publishers.AnyAsync(publisher => publisher.Name == name, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Publisher name already exists."));
        var item = request.Adapt<Publisher>();
        item.Name = name;
        item.Description = request.Description?.Trim();
        await publishers.AddAsync(item, cancellationToken);
        await publishers.CommitAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ApiResponseFactory.Success(item.Adapt<PublisherResponse>(), "Publisher created successfully."));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PublisherRequest request, CancellationToken cancellationToken)
    {
        var item = await publishers.GetOneAsync(publisher => publisher.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Publisher not found."));
        var name = request.Name.Trim();
        if (await publishers.AnyAsync(publisher => publisher.Id != id && publisher.Name == name, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Publisher name already exists."));
        request.Adapt(item);
        item.Name = name;
        item.Description = request.Description?.Trim();
        await publishers.CommitAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var item = await publishers.GetOneAsync(publisher => publisher.Id == id, cancellationToken: cancellationToken);
        if (item is null) return NotFound(ApiResponseFactory.Failure<object>("Publisher not found."));
        if (await books.AnyAsync(book => book.PublisherId == id, cancellationToken))
            return Conflict(ApiResponseFactory.Failure<object>("Publisher is in use.", ["Move or deactivate its books before deleting the publisher."]));
        publishers.Remove(item);
        await publishers.CommitAsync(cancellationToken);
        return NoContent();
    }
}
