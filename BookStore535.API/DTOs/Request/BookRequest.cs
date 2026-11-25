using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class BookRequest
{
    [Required, MinMaxCustomLength(2, 200)] public string Title { get; init; } = string.Empty;
    [StringLength(4000)] public string? Description { get; init; }
    [Required, StringLength(32)] public string BookCode { get; init; } = string.Empty;
    [Range(0, 1000000)] public decimal Price { get; init; }
    [Range(0, 1000000)] public decimal? OriginalPrice { get; init; }
    [Range(0, 1000000)] public int StockQuantity { get; init; }
    [Range(1800, 2200)] public int PublishedYear { get; init; }
    [StringLength(1000)] public string? CoverImageUrl { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsActive { get; init; } = true;
    [Range(1, int.MaxValue)] public int CategoryId { get; init; }
    [Range(1, int.MaxValue)] public int AuthorId { get; init; }
    [Range(1, int.MaxValue)] public int PublisherId { get; init; }
}
