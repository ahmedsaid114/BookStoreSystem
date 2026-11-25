using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookStore535.API.Migrations;

[DbContext(typeof(BookStore535.API.Data.ApplicationDbContext))]
[Migration("20261125000100_InitialBookStore")]
public sealed class InitialBookStore : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE [CustomerAccounts] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CustomerAccounts] PRIMARY KEY,
                [Name] nvarchar(120) NOT NULL,
                [ProfileImage] nvarchar(400) NULL,
                [Address] nvarchar(300) NULL,
                [CreatedUtc] datetime2 NOT NULL,
                [UserName] nvarchar(256) NOT NULL,
                [NormalizedUserName] nvarchar(256) NULL,
                [Email] nvarchar(256) NOT NULL,
                [NormalizedEmail] nvarchar(256) NOT NULL,
                [EmailConfirmed] bit NOT NULL,
                [PasswordHash] nvarchar(512) NOT NULL,
                [SecurityStamp] nvarchar(max) NULL,
                [ConcurrencyStamp] nvarchar(max) NULL,
                [PhoneNumber] nvarchar(max) NULL,
                [PhoneNumberConfirmed] bit NOT NULL,
                [TwoFactorEnabled] bit NOT NULL,
                [LockoutEnd] datetimeoffset NULL,
                [LockoutEnabled] bit NOT NULL,
                [AccessFailedCount] int NOT NULL
            );
            CREATE UNIQUE INDEX [IX_CustomerAccounts_NormalizedEmail] ON [CustomerAccounts]([NormalizedEmail]);
            CREATE UNIQUE INDEX [IX_CustomerAccounts_NormalizedUserName] ON [CustomerAccounts]([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

            CREATE TABLE [AspNetRoles] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AspNetRoles] PRIMARY KEY,
                [Name] nvarchar(256) NULL,
                [NormalizedName] nvarchar(256) NULL,
                [ConcurrencyStamp] nvarchar(max) NULL
            );
            CREATE UNIQUE INDEX [IX_AspNetRoles_NormalizedName] ON [AspNetRoles]([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

            CREATE TABLE [Categories] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Categories] PRIMARY KEY,
                [Name] nvarchar(100) NOT NULL,
                [Description] nvarchar(500) NULL,
                [ImageUrl] nvarchar(400) NULL,
                [IsActive] bit NOT NULL
            );
            CREATE UNIQUE INDEX [IX_Categories_Name] ON [Categories]([Name]);

            CREATE TABLE [Authors] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Authors] PRIMARY KEY,
                [Name] nvarchar(150) NOT NULL,
                [Biography] nvarchar(2000) NULL,
                [ImageUrl] nvarchar(400) NULL
            );
            CREATE INDEX [IX_Authors_Name] ON [Authors]([Name]);

            CREATE TABLE [Publishers] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Publishers] PRIMARY KEY,
                [Name] nvarchar(150) NOT NULL,
                [Description] nvarchar(1000) NULL,
                [WebsiteUrl] nvarchar(300) NULL
            );
            CREATE UNIQUE INDEX [IX_Publishers_Name] ON [Publishers]([Name]);

            CREATE TABLE [DiscountCodes] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DiscountCodes] PRIMARY KEY,
                [Code] nvarchar(40) NOT NULL,
                [PercentOff] decimal(5,2) NOT NULL,
                [MinimumOrderAmount] decimal(10,2) NOT NULL,
                [StartsUtc] datetime2 NULL,
                [ExpiresUtc] datetime2 NULL,
                [MaxUses] int NULL,
                [UsesCount] int NOT NULL,
                [IsActive] bit NOT NULL,
                [RowVersion] rowversion NOT NULL
            );
            CREATE UNIQUE INDEX [IX_DiscountCodes_Code] ON [DiscountCodes]([Code]);

            CREATE TABLE [AspNetUserClaims] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY,
                [UserId] int NOT NULL,
                [ClaimType] nvarchar(max) NULL,
                [ClaimValue] nvarchar(max) NULL,
                CONSTRAINT [FK_AspNetUserClaims_CustomerAccounts_UserId] FOREIGN KEY ([UserId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE CASCADE
            );
            CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims]([UserId]);

            CREATE TABLE [AspNetRoleClaims] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY,
                [RoleId] int NOT NULL,
                [ClaimType] nvarchar(max) NULL,
                [ClaimValue] nvarchar(max) NULL,
                CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
            );
            CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims]([RoleId]);

            CREATE TABLE [AspNetUserLogins] (
                [LoginProvider] nvarchar(128) NOT NULL,
                [ProviderKey] nvarchar(128) NOT NULL,
                [ProviderDisplayName] nvarchar(max) NULL,
                [UserId] int NOT NULL,
                CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
                CONSTRAINT [FK_AspNetUserLogins_CustomerAccounts_UserId] FOREIGN KEY ([UserId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE CASCADE
            );
            CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins]([UserId]);

            CREATE TABLE [AspNetUserRoles] (
                [UserId] int NOT NULL,
                [RoleId] int NOT NULL,
                CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
                CONSTRAINT [FK_AspNetUserRoles_CustomerAccounts_UserId] FOREIGN KEY ([UserId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles]([Id]) ON DELETE CASCADE
            );
            CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles]([RoleId]);

            CREATE TABLE [AspNetUserTokens] (
                [UserId] int NOT NULL,
                [LoginProvider] nvarchar(128) NOT NULL,
                [Name] nvarchar(128) NOT NULL,
                [Value] nvarchar(max) NULL,
                CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
                CONSTRAINT [FK_AspNetUserTokens_CustomerAccounts_UserId] FOREIGN KEY ([UserId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE CASCADE
            );

            CREATE TABLE [Books] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Books] PRIMARY KEY,
                [Title] nvarchar(200) NOT NULL,
                [Description] nvarchar(4000) NULL,
                [BookCode] nvarchar(32) NOT NULL,
                [Price] decimal(10,2) NOT NULL,
                [OriginalPrice] decimal(10,2) NULL,
                [StockQuantity] int NOT NULL,
                [PublishedYear] int NOT NULL,
                [CoverImageUrl] nvarchar(1000) NULL,
                [IsFeatured] bit NOT NULL,
                [IsActive] bit NOT NULL,
                [CreatedUtc] datetime2 NOT NULL,
                [RowVersion] rowversion NOT NULL,
                [CategoryId] int NOT NULL,
                [AuthorId] int NOT NULL,
                [PublisherId] int NOT NULL,
                CONSTRAINT [FK_Books_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id]) ON DELETE NO ACTION,
                CONSTRAINT [FK_Books_Authors_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]) ON DELETE NO ACTION,
                CONSTRAINT [FK_Books_Publishers_PublisherId] FOREIGN KEY ([PublisherId]) REFERENCES [Publishers]([Id]) ON DELETE NO ACTION
            );
            CREATE UNIQUE INDEX [IX_Books_BookCode] ON [Books]([BookCode]);
            CREATE INDEX [IX_Books_CategoryId_IsActive] ON [Books]([CategoryId], [IsActive]);
            CREATE INDEX [IX_Books_PublisherId_PublishedYear] ON [Books]([PublisherId], [PublishedYear]);
            CREATE INDEX [IX_Books_AuthorId] ON [Books]([AuthorId]);

            CREATE TABLE [BookReviews] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookReviews] PRIMARY KEY,
                [BookId] int NOT NULL,
                [CustomerAccountId] int NOT NULL,
                [Rating] int NOT NULL,
                [Comment] nvarchar(2000) NULL,
                [CreatedUtc] datetime2 NOT NULL,
                [IsApproved] bit NOT NULL,
                CONSTRAINT [FK_BookReviews_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_BookReviews_CustomerAccounts_CustomerAccountId] FOREIGN KEY ([CustomerAccountId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE NO ACTION
            );
            CREATE UNIQUE INDEX [IX_BookReviews_CustomerAccountId_BookId] ON [BookReviews]([CustomerAccountId], [BookId]);
            CREATE INDEX [IX_BookReviews_BookId] ON [BookReviews]([BookId]);

            CREATE TABLE [ShoppingCarts] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ShoppingCarts] PRIMARY KEY,
                [CustomerAccountId] int NOT NULL,
                [UpdatedUtc] datetime2 NOT NULL,
                CONSTRAINT [FK_ShoppingCarts_CustomerAccounts_CustomerAccountId] FOREIGN KEY ([CustomerAccountId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX [IX_ShoppingCarts_CustomerAccountId] ON [ShoppingCarts]([CustomerAccountId]);

            CREATE TABLE [Wishlists] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Wishlists] PRIMARY KEY,
                [CustomerAccountId] int NOT NULL,
                [UpdatedUtc] datetime2 NOT NULL,
                CONSTRAINT [FK_Wishlists_CustomerAccounts_CustomerAccountId] FOREIGN KEY ([CustomerAccountId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE CASCADE
            );
            CREATE UNIQUE INDEX [IX_Wishlists_CustomerAccountId] ON [Wishlists]([CustomerAccountId]);

            CREATE TABLE [BookOrders] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookOrders] PRIMARY KEY,
                [OrderNumber] nvarchar(40) NOT NULL,
                [CustomerAccountId] int NOT NULL,
                [Status] nvarchar(20) NOT NULL,
                [PaymentMethod] nvarchar(30) NOT NULL,
                [RecipientName] nvarchar(120) NOT NULL,
                [PhoneNumber] nvarchar(20) NOT NULL,
                [AddressLine] nvarchar(300) NOT NULL,
                [City] nvarchar(100) NOT NULL,
                [Subtotal] decimal(10,2) NOT NULL,
                [DiscountAmount] decimal(10,2) NOT NULL,
                [ShippingAmount] decimal(10,2) NOT NULL,
                [TaxAmount] decimal(10,2) NOT NULL,
                [TotalAmount] decimal(10,2) NOT NULL,
                [DiscountCode] nvarchar(40) NULL,
                [CreatedUtc] datetime2 NOT NULL,
                CONSTRAINT [FK_BookOrders_CustomerAccounts_CustomerAccountId] FOREIGN KEY ([CustomerAccountId]) REFERENCES [CustomerAccounts]([Id]) ON DELETE NO ACTION
            );
            CREATE UNIQUE INDEX [IX_BookOrders_OrderNumber] ON [BookOrders]([OrderNumber]);
            CREATE INDEX [IX_BookOrders_CustomerAccountId_CreatedUtc] ON [BookOrders]([CustomerAccountId], [CreatedUtc]);

            CREATE TABLE [CartItems] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CartItems] PRIMARY KEY,
                [ShoppingCartId] int NOT NULL,
                [BookId] int NOT NULL,
                [Quantity] int NOT NULL,
                [AddedUtc] datetime2 NOT NULL,
                CONSTRAINT [FK_CartItems_ShoppingCarts_ShoppingCartId] FOREIGN KEY ([ShoppingCartId]) REFERENCES [ShoppingCarts]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_CartItems_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books]([Id]) ON DELETE NO ACTION
            );
            CREATE UNIQUE INDEX [IX_CartItems_ShoppingCartId_BookId] ON [CartItems]([ShoppingCartId], [BookId]);
            CREATE INDEX [IX_CartItems_BookId] ON [CartItems]([BookId]);

            CREATE TABLE [WishlistItems] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WishlistItems] PRIMARY KEY,
                [WishlistId] int NOT NULL,
                [BookId] int NOT NULL,
                [AddedUtc] datetime2 NOT NULL,
                CONSTRAINT [FK_WishlistItems_Wishlists_WishlistId] FOREIGN KEY ([WishlistId]) REFERENCES [Wishlists]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_WishlistItems_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books]([Id]) ON DELETE NO ACTION
            );
            CREATE UNIQUE INDEX [IX_WishlistItems_WishlistId_BookId] ON [WishlistItems]([WishlistId], [BookId]);
            CREATE INDEX [IX_WishlistItems_BookId] ON [WishlistItems]([BookId]);

            CREATE TABLE [BookOrderItems] (
                [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookOrderItems] PRIMARY KEY,
                [BookOrderId] int NOT NULL,
                [BookId] int NOT NULL,
                [TitleSnapshot] nvarchar(200) NOT NULL,
                [BookCodeSnapshot] nvarchar(32) NOT NULL,
                [Quantity] int NOT NULL,
                [UnitPrice] decimal(10,2) NOT NULL,
                [LineTotal] decimal(10,2) NOT NULL,
                CONSTRAINT [FK_BookOrderItems_BookOrders_BookOrderId] FOREIGN KEY ([BookOrderId]) REFERENCES [BookOrders]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_BookOrderItems_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books]([Id]) ON DELETE NO ACTION
            );
            CREATE UNIQUE INDEX [IX_BookOrderItems_BookOrderId_BookId] ON [BookOrderItems]([BookOrderId], [BookId]);
            CREATE INDEX [IX_BookOrderItems_BookId] ON [BookOrderItems]([BookId]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DROP TABLE [BookOrderItems];
            DROP TABLE [CartItems];
            DROP TABLE [WishlistItems];
            DROP TABLE [BookReviews];
            DROP TABLE [BookOrders];
            DROP TABLE [ShoppingCarts];
            DROP TABLE [Wishlists];
            DROP TABLE [Books];
            DROP TABLE [DiscountCodes];
            DROP TABLE [Publishers];
            DROP TABLE [Authors];
            DROP TABLE [Categories];
            DROP TABLE [AspNetUserTokens];
            DROP TABLE [AspNetUserRoles];
            DROP TABLE [AspNetUserLogins];
            DROP TABLE [AspNetUserClaims];
            DROP TABLE [AspNetRoleClaims];
            DROP TABLE [AspNetRoles];
            DROP TABLE [CustomerAccounts];
            """);
    }
}
