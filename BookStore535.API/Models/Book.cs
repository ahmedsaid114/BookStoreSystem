using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore535.API.Models;

public sealed class Book
{
    public int Id { get; set; }

    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [StringLength(4000)] public string? Description { get; set; }
    [Required, StringLength(32)] public string BookCode { get; set; } = string.Empty;
    [Column(TypeName = "decimal(10,2)"), Range(0, 1000000)] public decimal Price { get; set; }
    [Column(TypeName = "decimal(10,2)"), Range(0, 1000000)] public decimal? OriginalPrice { get; set; }
    [Range(0, 1000000)] public int StockQuantity { get; set; }
    [Range(1800, 2200)] public int PublishedYear { get; set; }
    [StringLength(1000)] public string? CoverImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    [Timestamp] public byte[] RowVersion { get; set; } = [];

    [Range(1, int.MaxValue)] public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    [Range(1, int.MaxValue)] public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    [Range(1, int.MaxValue)] public int PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public ICollection<BookReview> Reviews { get; set; } = new List<BookReview>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    public ICollection<BookOrderItem> OrderItems { get; set; } = new List<BookOrderItem>();
}
