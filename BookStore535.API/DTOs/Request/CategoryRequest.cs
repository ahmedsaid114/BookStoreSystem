using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class CategoryRequest
{
    [Required, MinMaxCustomLength(2, 100)] public string Name { get; init; } = string.Empty;
    [StringLength(500)] public string? Description { get; init; }
    [StringLength(400)] public string? ImageUrl { get; init; }
    public bool IsActive { get; init; } = true;
}
