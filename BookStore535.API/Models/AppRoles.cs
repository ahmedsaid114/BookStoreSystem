namespace BookStore535.API.Models;

public static class AppRoles
{
    public const string Customer = "Customer";
    public const string Admin = "Admin";
    public const string SuperAdmin = "SuperAdmin";

    public static readonly string[] All = [Customer, Admin, SuperAdmin];
    public const string AdminAccess = Admin + "," + SuperAdmin;
}

