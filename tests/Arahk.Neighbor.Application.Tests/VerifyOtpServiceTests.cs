using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Arahk.Neighbor.Application.Tests;

public class VerifyOtpServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IOtpRepository> _otps = new();
    private readonly Mock<IClock> _clock = new();
    private readonly DateTimeOffset _now = new(2026, 9, 23, 10, 0, 0, TimeSpan.FromHours(7));

    private VerifyOtpService Sut() => new(_users.Object, _otps.Object, _clock.Object);

    private User UnverifiedUser()
    {
        EmailAddress.TryParse("you@email.com", out var em);
        return User.Create("มานี", em!, "HASH", true, _now);
    }

    [Fact]
    public async Task Verify_Success_MarksEmailVerified()
    {
        _clock.Setup(c => c.UtcNow).Returns(_now);
        var user = UnverifiedUser();
        var otp = EmailOtp.Create(user.Id, user.Email, "123456", _now);
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _otps.Setup(o => o.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otp);
        var result = await Sut().ExecuteAsync(new VerifyOtpRequest(user.Id, "123456"));
        result.Succeeded.Should().BeTrue();
        user.EmailVerified.Should().BeTrue();
    }

    [Fact]
    public async Task Verify_Wrong_ReturnsRemaining()
    {
        _clock.Setup(c => c.UtcNow).Returns(_now);
        var user = UnverifiedUser();
        var otp = EmailOtp.Create(user.Id, user.Email, "123456", _now);
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _otps.Setup(o => o.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otp);
        var result = await Sut().ExecuteAsync(new VerifyOtpRequest(user.Id, "000000"));
        result.Succeeded.Should().BeFalse();
        result.ErrorKey.Should().Be(ErrorKeys.OtpInvalidRemaining);
        result.RemainingAttempts.Should().Be(4);
    }

    [Fact]
    public async Task Verify_Expired()
    {
        var user = UnverifiedUser();
        var otp = EmailOtp.Create(user.Id, user.Email, "123456", _now);
        _clock.Setup(c => c.UtcNow).Returns(_now.AddMinutes(11));
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _otps.Setup(o => o.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otp);
        var result = await Sut().ExecuteAsync(new VerifyOtpRequest(user.Id, "123456"));
        result.ErrorKey.Should().Be(ErrorKeys.OtpExpired);
    }
}
