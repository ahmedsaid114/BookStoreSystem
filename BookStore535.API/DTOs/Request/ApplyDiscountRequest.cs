using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class ApplyDiscountRequest
{
    [Required, MinMaxCustomLength(3, 40)] public string Code { get; init; } = string.Empty;
}
