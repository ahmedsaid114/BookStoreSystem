using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class LoginRequest
{
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string Password { get; init; } = string.Empty;
}

