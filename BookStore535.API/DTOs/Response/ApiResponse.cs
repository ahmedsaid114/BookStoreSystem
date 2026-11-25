namespace BookStore535.API.DTOs.Response;

public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IEnumerable<string>? Errors { get; init; }
}

