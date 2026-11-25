using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore535.API.Models;

public static class BookOrderStatuses
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Shipped = "Shipped";
    public const string Delivered = "Delivered";
    public const string Cancelled = "Cancelled";
}

public sealed class BookOrder
{
    public int Id { get; set; }
    [Required, StringLength(40)] public string OrderNumber { get; set; } = string.Empty;
    public int CustomerAccountId { get; set; }
    public CustomerAccount CustomerAccount { get; set; } = null!;
    [Required, StringLength(20)] public string Status { get; set; } = BookOrderStatuses.Pending;
    [Required, StringLength(30)] public string PaymentMethod { get; set; } = "CashOnDelivery";
    [Required, StringLength(120)] public string RecipientName { get; set; } = string.Empty;
    [Required, StringLength(20)] public string PhoneNumber { get; set; } = string.Empty;
    [Required, StringLength(300)] public string AddressLine { get; set; } = string.Empty;
    [Required, StringLength(100)] public string City { get; set; } = string.Empty;
    [Column(TypeName = "decimal(10,2)")] public decimal Subtotal { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal DiscountAmount { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal ShippingAmount { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal TaxAmount { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal TotalAmount { get; set; }
    [StringLength(40)] public string? DiscountCode { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public ICollection<BookOrderItem> Items { get; set; } = new List<BookOrderItem>();
}

public sealed class BookOrderItem
{
    public int Id { get; set; }
    public int BookOrderId { get; set; }
    public BookOrder BookOrder { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    [Required, StringLength(200)] public string TitleSnapshot { get; set; } = string.Empty;
    [Required, StringLength(32)] public string BookCodeSnapshot { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal UnitPrice { get; set; }
    [Column(TypeName = "decimal(10,2)")] public decimal LineTotal { get; set; }
}
