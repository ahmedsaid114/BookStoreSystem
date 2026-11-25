using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class DiscountCodeRequest
{
    [Required, MinMaxCustomLength(3, 40)] public string Code { get; init; } = string.Empty;
    [Range(0.01, 100)] public decimal PercentOff { get; init; }
    [Range(0, 1000000)] public decimal MinimumOrderAmount { get; init; }
    public DateTime? StartsUtc { get; init; }
    public DateTime? ExpiresUtc { get; init; }
    [Range(1, int.MaxValue)] public int? MaxUses { get; init; }
    public bool IsActive { get; init; } = true;
}
