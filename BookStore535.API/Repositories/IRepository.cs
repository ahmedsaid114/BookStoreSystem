using System.Linq.Expressions;

namespace BookStore535.API.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        string[]? includes = null,
        bool isTracked = true,
        CancellationToken cancellationToken = default);
    Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        string[]? includes = null,
        bool isTracked = true,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> GetPageAsync(
        Expression<Func<TEntity, bool>>? filter,
        int skip,
        int take,
        Expression<Func<TEntity, object>> orderBy,
        string[]? includes = null,
        bool descending = false,
        CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}

