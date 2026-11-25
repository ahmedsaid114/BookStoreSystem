namespace BookStore535.API.DTOs.Response;

public sealed class BookResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string BookCode { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? OriginalPrice { get; init; }
    public int StockQuantity { get; init; }
    public int PublishedYear { get; init; }
    public string? CoverImageUrl { get; init; }
    public bool IsFeatured { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public int PublisherId { get; init; }
    public string PublisherName { get; init; } = string.Empty;
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
}
