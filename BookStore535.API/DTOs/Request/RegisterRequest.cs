using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class RegisterRequest
{
    [Required, MinMaxCustomLength(2, 60)] public string FirstName { get; init; } = string.Empty;
    [Required, MinMaxCustomLength(2, 60)] public string LastName { get; init; } = string.Empty;
    [Required, EmailAddress, StringLength(256)] public string Email { get; init; } = string.Empty;
    [Required, MinLength(8)] public string Password { get; init; } = string.Empty;
    [Required, Compare(nameof(Password))] public string ConfirmPassword { get; init; } = string.Empty;
    [Phone, StringLength(20)] public string? PhoneNumber { get; init; }
    [StringLength(300)] public string? Address { get; init; }
}

