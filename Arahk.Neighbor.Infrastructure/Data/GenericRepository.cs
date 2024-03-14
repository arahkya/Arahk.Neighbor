using Microsoft.EntityFrameworkCore;
using Arahk.Neighbor.Application.Data;
using Arahk.Neighbor.Domain.Common;
using System.Linq.Expressions;

namespace Arahk.Neighbor.Infrastructure.Data;

public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly NeighborDbContext _dbContext;

    public GenericRepository(NeighborDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().FindAsync([id], cancellationToken) ?? throw new Exception("Entity not found");
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public Task<TEntity> GetByConditionAsync(Expression<Func<TEntity, bool>> expCondition)
    {
        return _dbContext.Set<TEntity>().SingleAsync(expCondition);
    }
}