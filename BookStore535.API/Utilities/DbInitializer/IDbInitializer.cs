namespace BookStore535.API.Utilities.DbInitializer;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

