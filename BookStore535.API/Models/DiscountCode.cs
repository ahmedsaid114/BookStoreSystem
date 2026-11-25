using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore535.API.Models;

public sealed class DiscountCode
{
    public int Id { get; set; }
    [Required, StringLength(40)] public string Code { get; set; } = string.Empty;
    [Column(TypeName = "decimal(5,2)"), Range(0.01, 100)] public decimal PercentOff { get; set; }
    [Column(TypeName = "decimal(10,2)"), Range(0, 1000000)] public decimal MinimumOrderAmount { get; set; }
    public DateTime? StartsUtc { get; set; }
    public DateTime? ExpiresUtc { get; set; }
    public int? MaxUses { get; set; }
    public int UsesCount { get; set; }
    public bool IsActive { get; set; } = true;
    [Timestamp] public byte[] RowVersion { get; set; } = [];
}
