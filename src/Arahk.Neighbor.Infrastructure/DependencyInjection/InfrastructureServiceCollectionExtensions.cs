using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Infrastructure.Email;
using Arahk.Neighbor.Infrastructure.Excel;
using Arahk.Neighbor.Infrastructure.Persistence;
using Arahk.Neighbor.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Arahk.Neighbor.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryUserRepository>();
        services.AddSingleton<IUserRepository>(sp => sp.GetRequiredService<InMemoryUserRepository>());
        services.AddSingleton<InMemoryOtpRepository>();
        services.AddSingleton<IOtpRepository>(sp => sp.GetRequiredService<InMemoryOtpRepository>());
        services.AddSingleton<InMemoryHouseRepository>();
        services.AddSingleton<IHouseRepository>(sp => sp.GetRequiredService<InMemoryHouseRepository>());
        services.AddSingleton<InMemoryPermissionMasterRepository>();
        services.AddSingleton<IPermissionMasterRepository>(sp => sp.GetRequiredService<InMemoryPermissionMasterRepository>());
        services.AddSingleton<InMemoryRolePermissionRepository>();
        services.AddSingleton<IRolePermissionRepository>(sp => sp.GetRequiredService<InMemoryRolePermissionRepository>());
        services.AddSingleton<IHouseExcelParser, ClosedXmlHouseExcelParser>();
        services.AddSingleton<InMemoryEmailSender>();
        services.AddSingleton<IEmailSender>(sp => sp.GetRequiredService<InMemoryEmailSender>());
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IOtpGenerator, RandomOtpGenerator>();
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
