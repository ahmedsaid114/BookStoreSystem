namespace BookStore535.API.DTOs.Response;

public sealed class WishlistResponse
{
    public IReadOnlyList<BookResponse> Items { get; init; } = [];
    public int ItemCount => Items.Count;
}
