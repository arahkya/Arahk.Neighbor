using Arahk.Neighbor.Application.Data;
using Arahk.Neighbor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arahk.Neighbor.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<NeighborDbContext>((servicesProvider,opt) =>
        {
            string connectionString = servicesProvider.GetRequiredService<IConfiguration>().GetConnectionString("NeighborDb") ?? throw new Exception("Connection string not found");

            opt.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork, NeighborDbContext>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        return services;
    }
}
