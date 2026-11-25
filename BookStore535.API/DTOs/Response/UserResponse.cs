namespace BookStore535.API.DTOs.Response;

public sealed class UserResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
}

