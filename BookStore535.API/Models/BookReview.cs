using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.Models;

public sealed class BookReview
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int CustomerAccountId { get; set; }
    public CustomerAccount CustomerAccount { get; set; } = null!;
    [Range(1, 5)] public int Rating { get; set; }
    [StringLength(2000)] public string? Comment { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = true;
}
