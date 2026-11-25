using Microsoft.AspNetCore.Identity;

namespace BookStore535.API.Models;

public sealed class CustomerAccount : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public string? Address { get; set; }

    public DateTime CreatedUtc { get; set; }

}

