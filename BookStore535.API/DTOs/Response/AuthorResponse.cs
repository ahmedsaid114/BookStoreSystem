namespace BookStore535.API.DTOs.Response;

public sealed class AuthorResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Biography { get; init; }
    public string? ImageUrl { get; init; }
    public int BookCount { get; init; }
}
