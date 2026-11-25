namespace BookStore535.API.DTOs.Response;

public sealed class CartResponse
{
    public IReadOnlyList<CartItemResponse> Items { get; init; } = [];
    public int ItemCount { get; init; }
    public decimal Subtotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal Total { get; init; }
    public string? AppliedDiscountCode { get; init; }
}

public sealed class CartItemResponse
{
    public int Id { get; init; }
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string BookCode { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
