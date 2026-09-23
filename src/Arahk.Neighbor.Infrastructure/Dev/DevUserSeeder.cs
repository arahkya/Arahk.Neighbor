using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arahk.Neighbor.Infrastructure.Dev;

/// <summary>
/// Idempotent Development seed for a verified test user (login without OTP).
/// </summary>
public static class DevUserSeeder
{
    /// <summary>
    /// Seeds only when <paramref name="environment"/> is Development.
    /// Returns true if a new user was created; false if skipped (not Development or already present).
    /// </summary>
    public static async Task<bool> SeedIfDevelopmentAsync(
        IServiceProvider services,
        IHostEnvironment environment,
        CancellationToken ct = default)
    {
        if (!environment.IsDevelopment())
            return false;

        await using var scope = services.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();
        return await EnsureVerifiedUserAsync(users, hasher, clock, ct);
    }

    /// <summary>
    /// Creates the verified seed user if missing. Idempotent by email.
    /// Does not check environment — callers must gate with IsDevelopment (or tests).
    /// </summary>
    public static async Task<bool> EnsureVerifiedUserAsync(
        IUserRepository users,
        IPasswordHasher hasher,
        IClock clock,
        CancellationToken ct = default)
    {
        var existing = await users.GetByEmailAsync(DevSeedUser.Email, ct);
        if (existing is not null)
            return false;

        if (!EmailAddress.TryParse(DevSeedUser.Email, out var email) || email is null)
            throw new InvalidOperationException("Dev seed email is invalid.");

        if (!PhoneNumber.TryParse(DevSeedUser.Phone, out var phone) || phone is null)
            throw new InvalidOperationException("Dev seed phone is invalid.");

        var hash = hasher.Hash(DevSeedUser.Password);
        var user = User.Create(
            DevSeedUser.DisplayName,
            email,
            hash,
            termsAccepted: true,
            clock.UtcNow,
            phone);
        user.MarkEmailVerified();
        await users.AddAsync(user, ct);
        return true;
    }
}
