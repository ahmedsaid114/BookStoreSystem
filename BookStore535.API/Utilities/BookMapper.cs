namespace BookStore535.API.Utilities;

public static class BookMapper
{
    public static BookResponse ToResponse(Book book)
    {
        var reviews = book.Reviews.Where(review => review.IsApproved).ToArray();
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            BookCode = book.BookCode,
            Price = book.Price,
            OriginalPrice = book.OriginalPrice,
            StockQuantity = book.StockQuantity,
            PublishedYear = book.PublishedYear,
            CoverImageUrl = book.CoverImageUrl,
            IsFeatured = book.IsFeatured,
            CategoryId = book.CategoryId,
            CategoryName = book.Category?.Name ?? string.Empty,
            AuthorId = book.AuthorId,
            AuthorName = book.Author?.Name ?? string.Empty,
            PublisherId = book.PublisherId,
            PublisherName = book.Publisher?.Name ?? string.Empty,
            AverageRating = reviews.Length == 0 ? 0 : Math.Round((decimal)reviews.Average(review => review.Rating), 1),
            ReviewCount = reviews.Length
        };
    }
}
