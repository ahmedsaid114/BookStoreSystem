using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class AuthorRequest
{
    [Required, MinMaxCustomLength(2, 150)] public string Name { get; init; } = string.Empty;
    [StringLength(2000)] public string? Biography { get; init; }
    [StringLength(400)] public string? ImageUrl { get; init; }
}
