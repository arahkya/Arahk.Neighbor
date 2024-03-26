using Arahk.Neighbor.Application;
using Arahk.Neighbor.Application.Commmand.House;
using Arahk.Neighbor.Domain.Administrator;
using Arahk.Neighbor.Infrastructure;
using Arahk.Neighbor.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Arahk.Neighbor.Test;

public class HouseUnitTest
{
    private readonly IServiceProvider serviceProvider;

    public HouseUnitTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddApplication();
        services.AddInfrastructure();

        services.Where(p => p.ServiceType.FullName!.Contains("DbContext"))
            .ToList()
            .ForEach(p => services.Remove(p));

        services.AddDbContext<NeighborDbContext>(options =>
        {
            options.UseInMemoryDatabase("NeighborTest");
        });

        serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async void CreateHouseTest()
    {
        // Arrange
        string addressName = "89/86";
        IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
        CreateRequest request = new()
        {
            AddressName = addressName
        };

        // Action
        HouseEntity house = await mediator.Send(request);

        // Assert
        Assert.NotEqual(Guid.Empty, house.Id);
        Assert.Equal(addressName, house.AddressName);
    }

    [Fact]
    public async void CreateDuplicateHouseTest()
    {
        // Arrange        
        string addressName = "89/85";

        NeighborDbContext neighborDbContext = serviceProvider.GetRequiredService<NeighborDbContext>();
        neighborDbContext.Houses.Add(new HouseEntity { AddressName = addressName });
        await neighborDbContext.SaveChangesAsync();

        IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
        CreateRequest request = new()
        {
            AddressName = addressName
        };

        // Action
        Func<Task> createHouseTask = new(() => mediator.Send(request));

        // Assert
        await Assert.ThrowsAsync<Exception>(createHouseTask);
    }
}