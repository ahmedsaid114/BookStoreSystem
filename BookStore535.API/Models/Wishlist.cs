namespace BookStore535.API.Models;

public sealed class Wishlist
{
    public int Id { get; set; }
    public int CustomerAccountId { get; set; }
    public CustomerAccount CustomerAccount { get; set; } = null!;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
}

public sealed class WishlistItem
{
    public int Id { get; set; }
    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public DateTime AddedUtc { get; set; } = DateTime.UtcNow;
}
