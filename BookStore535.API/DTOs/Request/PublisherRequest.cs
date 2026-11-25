using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class PublisherRequest
{
    [Required, MinMaxCustomLength(2, 150)] public string Name { get; init; } = string.Empty;
    [StringLength(1000)] public string? Description { get; init; }
    [Url, StringLength(300)] public string? WebsiteUrl { get; init; }
}
