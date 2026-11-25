namespace BookStore535.API.DTOs.Response;

public sealed class PublisherResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? WebsiteUrl { get; init; }
    public int BookCount { get; init; }
}
