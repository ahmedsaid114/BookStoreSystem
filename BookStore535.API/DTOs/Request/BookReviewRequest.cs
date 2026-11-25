using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class BookReviewRequest
{
    [Range(1, 5)] public int Rating { get; init; }
    [StringLength(2000)] public string? Comment { get; init; }
}
