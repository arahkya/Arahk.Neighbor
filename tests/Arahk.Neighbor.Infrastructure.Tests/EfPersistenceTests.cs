using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;
using Arahk.Neighbor.Infrastructure.Persistence;
using Arahk.Neighbor.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Arahk.Neighbor.Infrastructure.Tests;

/// <summary>
/// Proves EF + SQLite repositories keep data after disposing the DbContext (process-restart stand-in).
/// Uses a shared in-memory SQLite connection so schema/data outlive each context instance.
/// </summary>
public class EfPersistenceTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<NeighborDbContext> _options;
    private readonly Guid _community = HouseConstants.DefaultCommunityId;

    public EfPersistenceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        _options = new DbContextOptionsBuilder<NeighborDbContext>()
            .UseSqlite(_connection)
            .Options;
    }

    public async Task InitializeAsync()
    {
        await using var db = new NeighborDbContext(_options);
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    private NeighborDbContext CreateContext() => new(_options);

    [Fact]
    public async Task Users_survive_new_context()
    {
        Guid userId;
        await using (var db = CreateContext())
        {
            var repo = new EfUserRepository(db);
            EmailAddress.TryParse("persist@example.com", out var email).Should().BeTrue();
            PhoneNumber.TryParse("0812345678", out var phone).Should().BeTrue();
            var user = User.Create("ทดสอบ", email!, "hash", true, DateTimeOffset.UtcNow, phone);
            user.MarkEmailVerified();
            await repo.AddAsync(user);
            userId = user.Id;
        }

        await using (var db = CreateContext())
        {
            var repo = new EfUserRepository(db);
            var byId = await repo.GetByIdAsync(userId);
            byId.Should().NotBeNull();
            byId!.DisplayName.Should().Be("ทดสอบ");
            byId.EmailVerified.Should().BeTrue();

            var byEmail = await repo.GetByEmailAsync("persist@example.com");
            byEmail!.Id.Should().Be(userId);

            var byPhone = await repo.GetByPhoneLocalAsync("0812345678");
            byPhone!.Id.Should().Be(userId);

            byId.UpdateProfile("ชื่อใหม่", null);
            await repo.UpdateAsync(byId);
        }

        await using (var db = CreateContext())
        {
            var repo = new EfUserRepository(db);
            var again = await repo.GetByIdAsync(userId);
            again!.DisplayName.Should().Be("ชื่อใหม่");
            again.PhoneLocal.Should().BeNull();
        }
    }

    [Fact]
    public async Task Houses_crud_survives_new_context()
    {
        Guid houseId;
        await using (var db = CreateContext())
        {
            var repo = new EfHouseRepository(db);
            var now = DateTimeOffset.UtcNow;
            var house = House.Create(_community, "12/1", "ซอย A", now);
            await repo.AddAsync(house);
            houseId = house.Id;
        }

        await using (var db = CreateContext())
        {
            var repo = new EfHouseRepository(db);
            (await repo.ListByCommunityAsync(_community)).Should().HaveCount(1);
            (await repo.FindByHouseNoAsync(_community, " 12/1 "))!.Id.Should().Be(houseId);

            var house = await repo.GetByIdAsync(_community, houseId);
            house!.Update("12/1", "ใหม่", DateTimeOffset.UtcNow);
            await repo.UpdateAsync(house);
        }

        await using (var db = CreateContext())
        {
            var repo = new EfHouseRepository(db);
            (await repo.GetByIdAsync(_community, houseId))!.Soi.Should().Be("ใหม่");
            await repo.DeleteAsync((await repo.GetByIdAsync(_community, houseId))!);
        }

        await using (var db = CreateContext())
        {
            var repo = new EfHouseRepository(db);
            (await repo.ListByCommunityAsync(_community)).Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Permission_masters_and_role_grants_survive_new_context()
    {
        await using (var db = CreateContext())
        {
            var masters = new EfPermissionMasterRepository(db);
            var grants = new EfRolePermissionRepository(db);
            var now = DateTimeOffset.UtcNow;
            await masters.UpsertAsync(
                PermissionMasterState.Create(_community, PermissionKeys.MasterHouses, true, now));
            await grants.UpsertAsync(
                RolePermissionGrant.Create(
                    _community, RoleKeys.Juristic, PermissionKeys.MasterHouses, true, now));
        }

        await using (var db = CreateContext())
        {
            var masters = new EfPermissionMasterRepository(db);
            var grants = new EfRolePermissionRepository(db);

            var master = await masters.GetAsync(_community, PermissionKeys.MasterHouses);
            master!.IsEnabledInSystem.Should().BeTrue();
            master.SetEnabled(false, DateTimeOffset.UtcNow);
            await masters.UpsertAsync(master);

            var grant = await grants.GetAsync(
                _community, RoleKeys.Juristic, PermissionKeys.MasterHouses);
            grant!.IsEnabled.Should().BeTrue();
            grant.SetEnabled(false, DateTimeOffset.UtcNow);
            await grants.UpsertAsync(grant);
        }

        await using (var db = CreateContext())
        {
            var masters = new EfPermissionMasterRepository(db);
            var grants = new EfRolePermissionRepository(db);
            (await masters.GetAsync(_community, PermissionKeys.MasterHouses))!
                .IsEnabledInSystem.Should().BeFalse();
            (await grants.GetAsync(_community, RoleKeys.Juristic, PermissionKeys.MasterHouses))!
                .IsEnabled.Should().BeFalse();
        }
    }

    [Fact]
    public async Task Otps_survive_new_context_and_invalidate()
    {
        Guid userId = Guid.NewGuid();
        await using (var db = CreateContext())
        {
            var repo = new EfOtpRepository(db);
            var otp = EmailOtp.Create(userId, "otp@example.com", "123456", DateTimeOffset.UtcNow);
            await repo.AddAsync(otp);
        }

        await using (var db = CreateContext())
        {
            var repo = new EfOtpRepository(db);
            var active = await repo.GetActiveByUserIdAsync(userId);
            active.Should().NotBeNull();
            active!.Code.Should().Be("123456");
            await repo.InvalidateActiveForUserAsync(userId);
        }

        await using (var db = CreateContext())
        {
            var repo = new EfOtpRepository(db);
            (await repo.GetActiveByUserIdAsync(userId)).Should().BeNull();
        }
    }

    [Fact]
    public async Task File_sqlite_survives_reopen()
    {
        var path = Path.Combine(Path.GetTempPath(), $"neighbor-test-{Guid.NewGuid():N}.db");
        try
        {
            var options = new DbContextOptionsBuilder<NeighborDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;

            Guid id;
            await using (var db = new NeighborDbContext(options))
            {
                await db.Database.EnsureCreatedAsync();
                var repo = new EfHouseRepository(db);
                var house = House.Create(_community, "99", null, DateTimeOffset.UtcNow);
                await repo.AddAsync(house);
                id = house.Id;
            }

            await using (var db = new NeighborDbContext(options))
            {
                var repo = new EfHouseRepository(db);
                (await repo.GetByIdAsync(_community, id))!.HouseNo.Should().Be("99");
            }
        }
        finally
        {
            TryDelete(path);
            TryDelete(path + "-shm");
            TryDelete(path + "-wal");
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // best-effort cleanup
        }
    }
}
