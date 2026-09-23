using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Arahk.Neighbor.Infrastructure.Persistence;

/// <summary>
/// Phase A: EnsureCreated (schema from model). Prefer Migrate() once migrations are introduced.
/// </summary>
public static class NeighborDbInitializer
{
    public static async Task EnsureCreatedAsync(
        IServiceProvider services,
        CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<NeighborDbContext>();
        await db.Database.EnsureCreatedAsync(ct);
    }
}
