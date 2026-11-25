namespace BookStore535.API.Utilities;

public static class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(T? data, string message) => new()
    {
        IsSuccess = true,
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Failure<T>(string message, IEnumerable<string>? errors = null) => new()
    {
        IsSuccess = false,
        Message = message,
        Errors = errors
    };
}

