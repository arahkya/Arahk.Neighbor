using Arahk.Neighbor.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Arahk.Neighbor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserService>();
        services.AddScoped<LoginService>();
        services.AddScoped<VerifyOtpService>();
        services.AddScoped<ResendOtpService>();
        services.AddScoped<HouseService>();
        return services;
    }
}
