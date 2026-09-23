using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Infrastructure.Email;
using Arahk.Neighbor.Infrastructure.Excel;
using Arahk.Neighbor.Infrastructure.Persistence;
using Arahk.Neighbor.Infrastructure.Persistence.Repositories;
using Arahk.Neighbor.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arahk.Neighbor.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public const string ConnectionStringName = "Neighbor";
    public const string DefaultSqliteConnection = "Data Source=neighbor.db";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? DefaultSqliteConnection;

        services.AddDbContext<NeighborDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IOtpRepository, EfOtpRepository>();
        services.AddScoped<IHouseRepository, EfHouseRepository>();
        services.AddScoped<IPermissionMasterRepository, EfPermissionMasterRepository>();
        services.AddScoped<IRolePermissionRepository, EfRolePermissionRepository>();

        services.AddSingleton<IHouseExcelParser, ClosedXmlHouseExcelParser>();
        services.AddSingleton<InMemoryEmailSender>();
        services.AddSingleton<IEmailSender>(sp => sp.GetRequiredService<InMemoryEmailSender>());
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IOtpGenerator, RandomOtpGenerator>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
