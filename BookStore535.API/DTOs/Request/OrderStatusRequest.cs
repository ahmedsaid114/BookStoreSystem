using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class OrderStatusRequest
{
    [Required, StringLength(20)] public string Status { get; init; } = string.Empty;
}
