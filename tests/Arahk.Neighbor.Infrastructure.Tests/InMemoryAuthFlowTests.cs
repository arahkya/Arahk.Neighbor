using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Infrastructure.Email;
using Arahk.Neighbor.Infrastructure.Persistence;
using Arahk.Neighbor.Infrastructure.Security;
using FluentAssertions;

namespace Arahk.Neighbor.Infrastructure.Tests;

public class InMemoryAuthFlowTests
{
    [Fact]
    public async Task Register_VerifyOtp_ThenLogin_WorksEndToEnd()
    {
        var users = new InMemoryUserRepository();
        var otps = new InMemoryOtpRepository();
        var email = new InMemoryEmailSender();
        var hasher = new Pbkdf2PasswordHasher();
        var clock = new SystemClock();
        var otpGen = new RandomOtpGenerator();

        var register = new RegisterUserService(users, otps, hasher, otpGen, email, clock);
        var verify = new VerifyOtpService(users, otps, clock);
        var login = new LoginService(users, hasher);

        var reg = await register.ExecuteAsync(new RegisterRequest(
            "มานี", "you@email.com", "0812345678", "Pass1234", "Pass1234", true));
        reg.Succeeded.Should().BeTrue();
        email.LastOtpCode.Should().NotBeNullOrEmpty();

        var verified = await verify.ExecuteAsync(new VerifyOtpRequest(reg.Data!.UserId, email.LastOtpCode!));
        verified.Succeeded.Should().BeTrue();

        var loggedIn = await login.ExecuteAsync(new LoginRequest(LoginMode.Email, "you@email.com", "Pass1234"));
        loggedIn.Succeeded.Should().BeTrue();
        loggedIn.Data!.EmailVerified.Should().BeTrue();

        var phoneLogin = await login.ExecuteAsync(new LoginRequest(LoginMode.Phone, "0812345678", "Pass1234"));
        phoneLogin.Succeeded.Should().BeTrue();
    }
}
