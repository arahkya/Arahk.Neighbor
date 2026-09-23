using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using Arahk.Neighbor.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Arahk.Neighbor.Application.Tests;

public class ResendOtpServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IOtpRepository> _otps = new();
    private readonly Mock<IOtpGenerator> _gen = new();
    private readonly Mock<IEmailSender> _email = new();
    private readonly Mock<IClock> _clock = new();
    private readonly DateTimeOffset _now = new(2026, 9, 23, 10, 0, 0, TimeSpan.FromHours(7));

    private ResendOtpService Sut()
    {
        _gen.Setup(g => g.GenerateSixDigitCode()).Returns("999888");
        return new ResendOtpService(_users.Object, _otps.Object, _gen.Object, _email.Object, _clock.Object);
    }

    [Fact]
    public async Task Resend_DuringCooldown_Fails()
    {
        _clock.Setup(c => c.UtcNow).Returns(_now);
        EmailAddress.TryParse("you@email.com", out var em);
        var user = User.Create("มานี", em!, "H", true, _now);
        var otp = EmailOtp.Create(user.Id, user.Email, "123456", _now);
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _otps.Setup(o => o.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otp);
        var result = await Sut().ExecuteAsync(new ResendOtpRequest(user.Id));
        result.Succeeded.Should().BeFalse();
        result.ErrorKey.Should().Be(ErrorKeys.OtpCooldown);
        result.CooldownSeconds.Should().Be(60);
    }

    [Fact]
    public async Task Resend_AfterCooldown_SendsNewCode()
    {
        EmailAddress.TryParse("you@email.com", out var em);
        var user = User.Create("มานี", em!, "H", true, _now);
        var otp = EmailOtp.Create(user.Id, user.Email, "123456", _now);
        _clock.Setup(c => c.UtcNow).Returns(_now.AddSeconds(61));
        _users.Setup(u => u.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _otps.Setup(o => o.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otp);
        var result = await Sut().ExecuteAsync(new ResendOtpRequest(user.Id));
        result.Succeeded.Should().BeTrue();
        _email.Verify(e => e.SendOtpAsync(user.Email, "999888", It.IsAny<CancellationToken>()), Times.Once);
        _otps.Verify(o => o.InvalidateActiveForUserAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
