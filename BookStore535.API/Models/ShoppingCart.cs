namespace BookStore535.API.Models;

public sealed class ShoppingCart
{
    public int Id { get; set; }
    public int CustomerAccountId { get; set; }
    public CustomerAccount CustomerAccount { get; set; } = null!;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

public sealed class CartItem
{
    public int Id { get; set; }
    public int ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime AddedUtc { get; set; } = DateTime.UtcNow;
}
