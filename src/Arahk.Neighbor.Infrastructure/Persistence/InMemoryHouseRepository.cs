using System.Collections.Concurrent;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;

namespace Arahk.Neighbor.Infrastructure.Persistence;

public class InMemoryHouseRepository : IHouseRepository
{
    private readonly ConcurrentDictionary<Guid, House> _byId = new();

    public Task<IReadOnlyList<House>> ListByCommunityAsync(Guid communityId, CancellationToken ct = default)
    {
        IReadOnlyList<House> list = _byId.Values
            .Where(h => h.CommunityId == communityId)
            .ToList();
        return Task.FromResult(list);
    }

    public Task<House?> GetByIdAsync(Guid communityId, Guid id, CancellationToken ct = default)
    {
        if (_byId.TryGetValue(id, out var house) && house.CommunityId == communityId)
            return Task.FromResult<House?>(house);
        return Task.FromResult<House?>(null);
    }

    public Task<House?> FindByHouseNoAsync(Guid communityId, string houseNo, CancellationToken ct = default)
    {
        var normalized = House.NormalizeHouseNo(houseNo);
        var house = _byId.Values.FirstOrDefault(h =>
            h.CommunityId == communityId &&
            string.Equals(h.HouseNo, normalized, StringComparison.Ordinal));
        return Task.FromResult(house);
    }

    public Task AddAsync(House house, CancellationToken ct = default)
    {
        if (!_byId.TryAdd(house.Id, house))
            throw new InvalidOperationException("House already exists.");
        return Task.CompletedTask;
    }

    public Task UpdateAsync(House house, CancellationToken ct = default)
    {
        _byId[house.Id] = house;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(House house, CancellationToken ct = default)
    {
        _byId.TryRemove(house.Id, out _);
        return Task.CompletedTask;
    }
}
