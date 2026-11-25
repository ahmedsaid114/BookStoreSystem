using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class UpdateProfileRequest
{
    [Required, MinMaxCustomLength(2, 120)] public string Name { get; init; } = string.Empty;
    [Phone, StringLength(20)] public string? PhoneNumber { get; init; }
}

