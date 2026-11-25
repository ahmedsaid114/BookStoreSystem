using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class AddCartItemRequest
{
    [Range(1, int.MaxValue)] public int BookId { get; init; }
    [Range(1, 99)] public int Quantity { get; init; } = 1;
}
