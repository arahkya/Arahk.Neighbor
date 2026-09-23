using Arahk.Neighbor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Persistence;

public class NeighborDbContext : DbContext
{
    public NeighborDbContext(DbContextOptions<NeighborDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<House> Houses => Set<House>();
    public DbSet<PermissionMasterState> PermissionMasters => Set<PermissionMasterState>();
    public DbSet<RolePermissionGrant> RolePermissionGrants => Set<RolePermissionGrant>();
    public DbSet<EmailOtp> EmailOtps => Set<EmailOtp>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NeighborDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
