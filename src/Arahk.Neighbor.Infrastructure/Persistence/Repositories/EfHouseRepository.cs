using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Persistence.Repositories;

public class EfHouseRepository : IHouseRepository
{
    private readonly NeighborDbContext _db;

    public EfHouseRepository(NeighborDbContext db) => _db = db;

    public async Task<IReadOnlyList<House>> ListByCommunityAsync(
        Guid communityId,
        CancellationToken ct = default)
    {
        return await _db.Houses
            .Where(h => h.CommunityId == communityId)
            .ToListAsync(ct);
    }

    public Task<House?> GetByIdAsync(Guid communityId, Guid id, CancellationToken ct = default) =>
        _db.Houses.FirstOrDefaultAsync(h => h.Id == id && h.CommunityId == communityId, ct);

    public Task<House?> FindByHouseNoAsync(Guid communityId, string houseNo, CancellationToken ct = default)
    {
        var normalized = House.NormalizeHouseNo(houseNo);
        return _db.Houses.FirstOrDefaultAsync(
            h => h.CommunityId == communityId && h.HouseNo == normalized,
            ct);
    }

    public async Task AddAsync(House house, CancellationToken ct = default)
    {
        _db.Houses.Add(house);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(House house, CancellationToken ct = default)
    {
        var entry = _db.Entry(house);
        if (entry.State == EntityState.Detached)
            _db.Houses.Update(house);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(House house, CancellationToken ct = default)
    {
        var entry = _db.Entry(house);
        if (entry.State == EntityState.Detached)
            _db.Houses.Attach(house);
        _db.Houses.Remove(house);
        await _db.SaveChangesAsync(ct);
    }
}
