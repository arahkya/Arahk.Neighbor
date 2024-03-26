using Arahk.Neighbor.Application.Data;
using Arahk.Neighbor.Domain.Administrator;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Data;

public class NeighborDbContext : DbContext, IUnitOfWork
{
    public DbSet<HouseEntity> Houses { get; set; }

    public IRepository<HouseEntity> HouseRepository { get; private set; }

    public NeighborDbContext(DbContextOptions<NeighborDbContext> options) : base(options)
    {
        HouseRepository = new GenericRepository<HouseEntity>(this);
    }

}