using System.ComponentModel.DataAnnotations;

namespace BookStore535.API.Models;

public sealed class Author
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [StringLength(2000)] public string? Biography { get; set; }
    [StringLength(400)] public string? ImageUrl { get; set; }
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
