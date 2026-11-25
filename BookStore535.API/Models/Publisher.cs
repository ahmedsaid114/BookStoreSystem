using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.Models;

public sealed class Publisher
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [StringLength(1000)] public string? Description { get; set; }
    [StringLength(300)] public string? WebsiteUrl { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
