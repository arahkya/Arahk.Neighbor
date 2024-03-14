using System.Linq.Expressions;
using Arahk.Neighbor.Domain.Common;

namespace Arahk.Neighbor.Application.Data
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TEntity> GetByConditionAsync(Expression<Func<TEntity, bool>> expCondition);
    }
}