using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class UpdateCartItemRequest
{
    [Range(1, 99)] public int Quantity { get; init; }
}
