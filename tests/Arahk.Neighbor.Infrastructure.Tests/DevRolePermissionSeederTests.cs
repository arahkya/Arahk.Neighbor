using Arahk.Neighbor.Domain.Catalog;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Infrastructure.Dev;
using Arahk.Neighbor.Infrastructure.Persistence;
using Arahk.Neighbor.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Arahk.Neighbor.Infrastructure.Tests;

public class DevRolePermissionSeederTests
{
    private readonly Guid _community = HouseConstants.DefaultCommunityId;

    [Fact]
    public async Task EnsureSeed_is_idempotent_and_seeds_defaults()
    {
        var masters = new InMemoryPermissionMasterRepository();
        var grants = new InMemoryRolePermissionRepository();
        var clock = new SystemClock();

        var first = await DevRolePermissionSeeder.EnsureSeedAsync(masters, grants, clock, _community);
        first.Should().BeTrue();

        var masterList = await masters.ListByCommunityAsync(_community);
        masterList.Should().HaveCount(PermissionCatalog.All.Count);
        masterList.Should().OnlyContain(m => m.IsEnabledInSystem);

        var grantList = await grants.ListByCommunityAsync(_community);
        grantList.Should().HaveCount(RoleCatalog.All.Count * PermissionCatalog.All.Count);

        var juristicPackages = await grants.GetAsync(_community, RoleKeys.Juristic, PermissionKeys.MenuPackages);
        juristicPackages!.IsEnabled.Should().BeTrue();
        var residentPackages = await grants.GetAsync(_community, RoleKeys.Resident, PermissionKeys.MenuPackages);
        residentPackages!.IsEnabled.Should().BeFalse();

        // Mutate then re-seed — must not overwrite
        await masters.UpsertAsync(
            PermissionMasterState.Create(_community, PermissionKeys.MasterRoles, false, clock.UtcNow));
        var second = await DevRolePermissionSeeder.EnsureSeedAsync(masters, grants, clock, _community);
        second.Should().BeFalse();

        var masterRoles = await masters.GetAsync(_community, PermissionKeys.MasterRoles);
        masterRoles!.IsEnabledInSystem.Should().BeFalse();
    }

    [Fact]
    public async Task SeedIfDevelopment_skips_non_development()
    {
        var services = new ServiceCollectionStub();
        var env = new HostEnvironmentStub { EnvironmentName = Environments.Production };
        var seeded = await DevRolePermissionSeeder.SeedIfDevelopmentAsync(services, env, _community);
        seeded.Should().BeFalse();
    }

    private sealed class HostEnvironmentStub : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "test";
        public string ContentRootPath { get; set; } = "/";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class ServiceCollectionStub : IServiceProvider
    {
        public object? GetService(Type serviceType) =>
            throw new InvalidOperationException("Should not resolve in Production.");
    }
}
