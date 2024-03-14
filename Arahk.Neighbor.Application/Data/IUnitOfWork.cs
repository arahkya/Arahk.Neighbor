using Arahk.Neighbor.Domain.Administrative;

namespace Arahk.Neighbor.Application.Data
{
    public interface IUnitOfWork
    {
        IRepository<HouseEntity> HouseRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}