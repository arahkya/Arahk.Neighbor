using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Arahk.Neighbor.Application.Tests;

public class LoginServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly DateTimeOffset _now = new(2026, 9, 23, 10, 0, 0, TimeSpan.FromHours(7));

    private LoginService Sut() => new(_users.Object, _hasher.Object);

    private User VerifiedUser()
    {
        EmailAddress.TryParse("you@email.com", out var em);
        PhoneNumber.TryParse("0812345678", out var ph);
        var u = User.Create("มานี", em!, "HASH", true, _now, ph);
        u.MarkEmailVerified();
        return u;
    }

    [Fact]
    public async Task Login_Email_Success()
    {
        var user = VerifiedUser();
        _users.Setup(u => u.GetByEmailAsync("you@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Pass1234", "HASH")).Returns(true);
        var result = await Sut().ExecuteAsync(new LoginRequest(LoginMode.Email, "you@email.com", "Pass1234"));
        result.Succeeded.Should().BeTrue();
        result.Data!.DisplayName.Should().Be("มานี");
    }

    [Fact]
    public async Task Login_Email_WrongPassword_GenericError()
    {
        var user = VerifiedUser();
        _users.Setup(u => u.GetByEmailAsync("you@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var result = await Sut().ExecuteAsync(new LoginRequest(LoginMode.Email, "you@email.com", "wrong"));
        result.ErrorKey.Should().Be(ErrorKeys.LoginCredentials);
    }

    [Fact]
    public async Task Login_Phone_NotLinked()
    {
        _users.Setup(u => u.GetByPhoneLocalAsync("0812345678", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var result = await Sut().ExecuteAsync(new LoginRequest(LoginMode.Phone, "0812345678", "Pass1234"));
        result.ErrorKey.Should().Be(ErrorKeys.LoginPhoneNotLinked);
    }

    [Fact]
    public async Task Login_Phone_Success()
    {
        var user = VerifiedUser();
        _users.Setup(u => u.GetByPhoneLocalAsync("0812345678", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("Pass1234", "HASH")).Returns(true);
        var result = await Sut().ExecuteAsync(new LoginRequest(LoginMode.Phone, "081-234-5678", "Pass1234"));
        result.Succeeded.Should().BeTrue();
    }
}
