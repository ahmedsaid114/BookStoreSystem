namespace BookStore535.API.DTOs.Response;

public sealed class DiscountCodeResponse
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public decimal PercentOff { get; init; }
    public decimal MinimumOrderAmount { get; init; }
    public DateTime? StartsUtc { get; init; }
    public DateTime? ExpiresUtc { get; init; }
    public int? MaxUses { get; init; }
    public int UsesCount { get; init; }
    public bool IsActive { get; init; }
}
