using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class AddWishlistItemRequest
{
    [Range(1, int.MaxValue)] public int BookId { get; init; }
}
