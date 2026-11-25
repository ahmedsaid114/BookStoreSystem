namespace BookStore535.API.DTOs.Response;

public sealed class BookReviewResponse
{
    public int Id { get; init; }
    public int BookId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public DateTime CreatedUtc { get; init; }
}
