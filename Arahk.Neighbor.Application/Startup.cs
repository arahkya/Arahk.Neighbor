using Arahk.Neighbor.Application.Commmand.House;
using Arahk.Neighbor.Application.Utils;
using Arahk.Neighbor.Domain.Administrative;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Arahk.Neighbor.Application;

public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(typeof(Startup).Assembly);
            
        });
        services.AddTransient<IPipelineBehavior<CreateRequest, HouseEntity>, CheckDuplicatePipeline>();
        
        services.AddScoped<CheckDuplicatePipeline>();
        services.AddScoped<HouseDuplicateChecker>();
        return services;
    }
}

