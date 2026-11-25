namespace BookStore535.API.DTOs.Response;

public sealed class OrderResponse
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string RecipientName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string AddressLine { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public decimal Subtotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal ShippingAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string? DiscountCode { get; init; }
    public DateTime CreatedUtc { get; init; }
    public IReadOnlyList<OrderItemResponse> Items { get; init; } = [];
}

public sealed class OrderItemResponse
{
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string BookCode { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
