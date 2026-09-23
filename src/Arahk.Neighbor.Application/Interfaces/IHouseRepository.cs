using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Application.Interfaces;

public interface IHouseRepository
{
    Task<IReadOnlyList<House>> ListByCommunityAsync(Guid communityId, CancellationToken ct = default);
    Task<House?> GetByIdAsync(Guid communityId, Guid id, CancellationToken ct = default);
    Task<House?> FindByHouseNoAsync(Guid communityId, string houseNo, CancellationToken ct = default);
    Task AddAsync(House house, CancellationToken ct = default);
    Task UpdateAsync(House house, CancellationToken ct = default);
    Task DeleteAsync(House house, CancellationToken ct = default);
}
