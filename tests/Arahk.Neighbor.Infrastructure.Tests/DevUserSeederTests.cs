using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Infrastructure.Dev;
using Arahk.Neighbor.Infrastructure.Persistence;
using Arahk.Neighbor.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Arahk.Neighbor.Infrastructure.Tests;

public class DevUserSeederTests
{
    private static (InMemoryUserRepository Users, Pbkdf2PasswordHasher Hasher, SystemClock Clock) CreateDeps()
    {
        var users = new InMemoryUserRepository();
        var hasher = new Pbkdf2PasswordHasher();
        var clock = new SystemClock();
        return (users, hasher, clock);
    }

    [Fact]
    public async Task EnsureVerifiedUser_CreatesVerifiedUser_Once()
    {
        var (users, hasher, clock) = CreateDeps();

        var created = await DevUserSeeder.EnsureVerifiedUserAsync(users, hasher, clock);
        created.Should().BeTrue();

        var user = await users.GetByEmailAsync(DevSeedUser.Email);
        user.Should().NotBeNull();
        user!.DisplayName.Should().Be(DevSeedUser.DisplayName);
        user.EmailVerified.Should().BeTrue();
        user.PhoneLocal.Should().Be(DevSeedUser.Phone);
        user.PasswordHash.Should().NotBe(DevSeedUser.Password);
        hasher.Verify(DevSeedUser.Password, user.PasswordHash).Should().BeTrue();

        var again = await DevUserSeeder.EnsureVerifiedUserAsync(users, hasher, clock);
        again.Should().BeFalse();

        var byPhone = await users.GetByPhoneLocalAsync(DevSeedUser.Phone);
        byPhone.Should().NotBeNull();
        byPhone!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task EnsureVerifiedUser_AllowsLogin_ByEmailAndPhone()
    {
        var (users, hasher, clock) = CreateDeps();
        await DevUserSeeder.EnsureVerifiedUserAsync(users, hasher, clock);
        var login = new LoginService(users, hasher);

        var emailLogin = await login.ExecuteAsync(
            new LoginRequest(LoginMode.Email, DevSeedUser.Email, DevSeedUser.Password));
        emailLogin.Succeeded.Should().BeTrue();
        emailLogin.Data!.EmailVerified.Should().BeTrue();

        var phoneLogin = await login.ExecuteAsync(
            new LoginRequest(LoginMode.Phone, DevSeedUser.Phone, DevSeedUser.Password));
        phoneLogin.Succeeded.Should().BeTrue();
        phoneLogin.Data!.EmailVerified.Should().BeTrue();
    }

    [Fact]
    public async Task SeedIfDevelopment_Skips_WhenNotDevelopment()
    {
        var users = new InMemoryUserRepository();
        var services = new ServiceCollection()
            .AddSingleton<IUserRepository>(users)
            .AddSingleton(users)
            .AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>()
            .AddSingleton<IClock, SystemClock>()
            .BuildServiceProvider();

        var created = await DevUserSeeder.SeedIfDevelopmentAsync(
            services,
            new StubHostEnvironment(Environments.Production));

        created.Should().BeFalse();
        (await users.GetByEmailAsync(DevSeedUser.Email)).Should().BeNull();
    }

    [Fact]
    public async Task SeedIfDevelopment_Seeds_WhenDevelopment()
    {
        var users = new InMemoryUserRepository();
        var services = new ServiceCollection()
            .AddSingleton<IUserRepository>(users)
            .AddSingleton(users)
            .AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>()
            .AddSingleton<IClock, SystemClock>()
            .BuildServiceProvider();

        var created = await DevUserSeeder.SeedIfDevelopmentAsync(
            services,
            new StubHostEnvironment(Environments.Development));

        created.Should().BeTrue();
        var user = await users.GetByEmailAsync(DevSeedUser.Email);
        user.Should().NotBeNull();
        user!.EmailVerified.Should().BeTrue();
    }

    private sealed class StubHostEnvironment : IHostEnvironment
    {
        public StubHostEnvironment(string environmentName) => EnvironmentName = environmentName;

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "Arahk.Neighbor.Tests";
        public string ContentRootPath { get; set; } = "/";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
