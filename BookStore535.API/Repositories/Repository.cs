using System.Linq.Expressions;
using BookStore535.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore535.API.Repositories;

public sealed class Repository<TEntity>(ApplicationDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _set = context.Set<TEntity>();

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await _set.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        string[]? includes = null,
        bool isTracked = true,
        CancellationToken cancellationToken = default) =>
        await BuildQuery(filter, includes, isTracked).ToListAsync(cancellationToken);

    public Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        string[]? includes = null,
        bool isTracked = true,
        CancellationToken cancellationToken = default) =>
        BuildQuery(filter, includes, isTracked).FirstOrDefaultAsync(cancellationToken);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default) =>
        filter is null ? _set.CountAsync(cancellationToken) : _set.CountAsync(filter, cancellationToken);

    public async Task<IReadOnlyList<TEntity>> GetPageAsync(
        Expression<Func<TEntity, bool>>? filter,
        int skip,
        int take,
        Expression<Func<TEntity, object>> orderBy,
        string[]? includes = null,
        bool descending = false,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(filter, includes, false);
        IOrderedQueryable<TEntity> ordered = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        return await ordered.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) =>
        _set.AnyAsync(filter, cancellationToken);

    public void Update(TEntity entity) => _set.Update(entity);
    public void Remove(TEntity entity) => _set.Remove(entity);
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);

    private IQueryable<TEntity> BuildQuery(
        Expression<Func<TEntity, bool>>? filter,
        IEnumerable<string>? includes,
        bool isTracked)
    {
        IQueryable<TEntity> query = _set;
        if (filter is not null) query = query.Where(filter);
        if (includes is not null)
        {
            foreach (var include in includes.Where(name => !string.IsNullOrWhiteSpace(name)))
                query = query.Include(include);
        }
        return isTracked ? query : query.AsNoTracking();
    }
}

