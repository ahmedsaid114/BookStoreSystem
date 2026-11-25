using BookStore535.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore535.API.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<BookReview> BookReviews => Set<BookReview>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
    public DbSet<BookOrder> BookOrders => Set<BookOrder>();
    public DbSet<BookOrderItem> BookOrderItems => Set<BookOrderItem>();

    public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
    public DbSet<CustomerAccount> Users => Set<CustomerAccount>();
    public DbSet<IdentityRole<int>> Roles => Set<IdentityRole<int>>();
    public DbSet<IdentityUserRole<int>> UserRoles => Set<IdentityUserRole<int>>();
    public DbSet<IdentityUserClaim<int>> UserClaims => Set<IdentityUserClaim<int>>();
    public DbSet<IdentityRoleClaim<int>> RoleClaims => Set<IdentityRoleClaim<int>>();
    public DbSet<IdentityUserLogin<int>> UserLogins => Set<IdentityUserLogin<int>>();
    public DbSet<IdentityUserToken<int>> UserTokens => Set<IdentityUserToken<int>>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureIdentity(modelBuilder);

        modelBuilder.Entity<Book>().HasIndex(book => book.BookCode).IsUnique();
        modelBuilder.Entity<Book>().HasIndex(book => new { book.CategoryId, book.IsActive });
        modelBuilder.Entity<Book>().HasIndex(book => new { book.PublisherId, book.PublishedYear });
        modelBuilder.Entity<Book>().Property(book => book.Price).HasPrecision(10, 2);
        modelBuilder.Entity<Book>().Property(book => book.OriginalPrice).HasPrecision(10, 2);
        modelBuilder.Entity<Book>().HasOne(book => book.Category).WithMany(category => category.Books)
            .HasForeignKey(book => book.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Book>().HasOne(book => book.Author).WithMany(author => author.Books)
            .HasForeignKey(book => book.AuthorId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Book>().HasOne(book => book.Publisher).WithMany(publisher => publisher.Books)
            .HasForeignKey(book => book.PublisherId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>().HasIndex(category => category.Name).IsUnique();
        modelBuilder.Entity<Author>().HasIndex(author => author.Name);
        modelBuilder.Entity<Publisher>().HasIndex(publisher => publisher.Name).IsUnique();

        modelBuilder.Entity<BookReview>().HasIndex(review => new { review.CustomerAccountId, review.BookId }).IsUnique();
        modelBuilder.Entity<BookReview>().HasOne(review => review.Book).WithMany(book => book.Reviews)
            .HasForeignKey(review => review.BookId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BookReview>().HasOne(review => review.CustomerAccount).WithMany()
            .HasForeignKey(review => review.CustomerAccountId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShoppingCart>().HasIndex(cart => cart.CustomerAccountId).IsUnique();
        modelBuilder.Entity<ShoppingCart>().HasOne(cart => cart.CustomerAccount).WithMany()
            .HasForeignKey(cart => cart.CustomerAccountId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CartItem>().HasIndex(item => new { item.ShoppingCartId, item.BookId }).IsUnique();
        modelBuilder.Entity<CartItem>().HasOne(item => item.ShoppingCart).WithMany(cart => cart.Items)
            .HasForeignKey(item => item.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CartItem>().HasOne(item => item.Book).WithMany(book => book.CartItems)
            .HasForeignKey(item => item.BookId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Wishlist>().HasIndex(list => list.CustomerAccountId).IsUnique();
        modelBuilder.Entity<Wishlist>().HasOne(list => list.CustomerAccount).WithMany()
            .HasForeignKey(list => list.CustomerAccountId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<WishlistItem>().HasIndex(item => new { item.WishlistId, item.BookId }).IsUnique();
        modelBuilder.Entity<WishlistItem>().HasOne(item => item.Wishlist).WithMany(list => list.Items)
            .HasForeignKey(item => item.WishlistId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<WishlistItem>().HasOne(item => item.Book).WithMany(book => book.WishlistItems)
            .HasForeignKey(item => item.BookId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DiscountCode>().HasIndex(code => code.Code).IsUnique();
        modelBuilder.Entity<DiscountCode>().Property(code => code.PercentOff).HasPrecision(5, 2);
        modelBuilder.Entity<DiscountCode>().Property(code => code.MinimumOrderAmount).HasPrecision(10, 2);

        modelBuilder.Entity<BookOrder>().HasIndex(order => order.OrderNumber).IsUnique();
        modelBuilder.Entity<BookOrder>().HasIndex(order => new { order.CustomerAccountId, order.CreatedUtc });
        modelBuilder.Entity<BookOrder>().HasOne(order => order.CustomerAccount).WithMany()
            .HasForeignKey(order => order.CustomerAccountId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BookOrderItem>().HasOne(item => item.BookOrder).WithMany(order => order.Items)
            .HasForeignKey(item => item.BookOrderId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BookOrderItem>().HasOne(item => item.Book).WithMany(book => book.OrderItems)
            .HasForeignKey(item => item.BookId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BookOrderItem>().HasIndex(item => new { item.BookOrderId, item.BookId }).IsUnique();
        modelBuilder.Entity<BookOrder>().Property(order => order.Subtotal).HasPrecision(10, 2);
        modelBuilder.Entity<BookOrder>().Property(order => order.DiscountAmount).HasPrecision(10, 2);
        modelBuilder.Entity<BookOrder>().Property(order => order.ShippingAmount).HasPrecision(10, 2);
        modelBuilder.Entity<BookOrder>().Property(order => order.TaxAmount).HasPrecision(10, 2);
        modelBuilder.Entity<BookOrder>().Property(order => order.TotalAmount).HasPrecision(10, 2);
        modelBuilder.Entity<BookOrderItem>().Property(item => item.UnitPrice).HasPrecision(10, 2);
        modelBuilder.Entity<BookOrderItem>().Property(item => item.LineTotal).HasPrecision(10, 2);
    }

    private static void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerAccount>().ToTable("CustomerAccounts");
        modelBuilder.Entity<CustomerAccount>().Property(user => user.Name).HasMaxLength(120).IsRequired();
        modelBuilder.Entity<CustomerAccount>().Property(user => user.ProfileImage).HasMaxLength(400);
        modelBuilder.Entity<CustomerAccount>().Property(user => user.Address).HasMaxLength(300);
        modelBuilder.Entity<CustomerAccount>().Property(user => user.UserName).HasMaxLength(256).IsRequired();
        modelBuilder.Entity<CustomerAccount>().Property(user => user.NormalizedUserName).HasMaxLength(256);
        modelBuilder.Entity<CustomerAccount>().Property(user => user.Email).HasMaxLength(256).IsRequired();
        modelBuilder.Entity<CustomerAccount>().Property(user => user.NormalizedEmail).HasMaxLength(256).IsRequired();
        modelBuilder.Entity<CustomerAccount>().Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        modelBuilder.Entity<CustomerAccount>().Property(user => user.ConcurrencyStamp).IsConcurrencyToken();
        modelBuilder.Entity<CustomerAccount>().HasIndex(user => user.NormalizedUserName).IsUnique()
            .HasFilter("[NormalizedUserName] IS NOT NULL");
        modelBuilder.Entity<CustomerAccount>().HasIndex(user => user.NormalizedEmail).IsUnique();

        modelBuilder.Entity<IdentityRole<int>>().ToTable("AspNetRoles");
        modelBuilder.Entity<IdentityRole<int>>().Property(role => role.Name).HasMaxLength(256);
        modelBuilder.Entity<IdentityRole<int>>().Property(role => role.NormalizedName).HasMaxLength(256);
        modelBuilder.Entity<IdentityRole<int>>().Property(role => role.ConcurrencyStamp).IsConcurrencyToken();
        modelBuilder.Entity<IdentityRole<int>>().HasIndex(role => role.NormalizedName).IsUnique()
            .HasFilter("[NormalizedName] IS NOT NULL");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("AspNetUserRoles").HasKey(link => new { link.UserId, link.RoleId });
        modelBuilder.Entity<IdentityUserRole<int>>().HasOne<IdentityRole<int>>().WithMany()
            .HasForeignKey(link => link.RoleId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<IdentityUserRole<int>>().HasOne<CustomerAccount>().WithMany()
            .HasForeignKey(link => link.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("AspNetUserClaims");
        modelBuilder.Entity<IdentityUserClaim<int>>().HasOne<CustomerAccount>().WithMany()
            .HasForeignKey(claim => claim.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("AspNetRoleClaims");
        modelBuilder.Entity<IdentityRoleClaim<int>>().HasOne<IdentityRole<int>>().WithMany()
            .HasForeignKey(claim => claim.RoleId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("AspNetUserLogins")
            .HasKey(login => new { login.LoginProvider, login.ProviderKey });
        modelBuilder.Entity<IdentityUserLogin<int>>().Property(login => login.LoginProvider).HasMaxLength(128);
        modelBuilder.Entity<IdentityUserLogin<int>>().Property(login => login.ProviderKey).HasMaxLength(128);
        modelBuilder.Entity<IdentityUserLogin<int>>().HasOne<CustomerAccount>().WithMany()
            .HasForeignKey(login => login.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("AspNetUserTokens")
            .HasKey(token => new { token.UserId, token.LoginProvider, token.Name });
        modelBuilder.Entity<IdentityUserToken<int>>().Property(token => token.LoginProvider).HasMaxLength(128);
        modelBuilder.Entity<IdentityUserToken<int>>().Property(token => token.Name).HasMaxLength(128);
        modelBuilder.Entity<IdentityUserToken<int>>().HasOne<CustomerAccount>().WithMany()
            .HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
