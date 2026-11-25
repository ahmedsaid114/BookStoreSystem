using Microsoft.AspNetCore.Mvc;

namespace BookStore535.API.Areas.Customer.Controllers;

[Area(CD.CUSTOMER_AREA)]
[Route("api/[area]/[controller]")]
[ApiController]
public sealed class PublishersController(IRepository<Publisher> publishers) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await publishers.GetAllAsync(includes: ["Books"], isTracked: false, cancellationToken: cancellationToken);
        var response = items.OrderBy(publisher => publisher.Name).Select(publisher => new PublisherResponse
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Description = publisher.Description,
            WebsiteUrl = publisher.WebsiteUrl,
            BookCount = publisher.Books.Count(book => book.IsActive)
        }).ToList();
        return Ok(ApiResponseFactory.Success<IReadOnlyList<PublisherResponse>>(response, "Publishers returned successfully."));
    }
}
