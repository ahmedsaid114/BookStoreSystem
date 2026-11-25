using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.Models;

public sealed class Category
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [StringLength(500)] public string? Description { get; set; }
    [StringLength(400)] public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
