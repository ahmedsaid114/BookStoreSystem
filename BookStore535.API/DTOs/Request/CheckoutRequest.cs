using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.DTOs.Request;

public sealed class CheckoutRequest
{
    [Required, MinMaxCustomLength(2, 120)] public string RecipientName { get; init; } = string.Empty;
    [Required, Phone, StringLength(20)] public string PhoneNumber { get; init; } = string.Empty;
    [Required, MinMaxCustomLength(5, 300)] public string AddressLine { get; init; } = string.Empty;
    [Required, MinMaxCustomLength(2, 100)] public string City { get; init; } = string.Empty;
    [StringLength(40)] public string? DiscountCode { get; init; }
    [Required, StringLength(30)] public string PaymentMethod { get; init; } = "CashOnDelivery";
}
