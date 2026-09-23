namespace Arahk.Neighbor.Application.Interfaces;

public interface IEmailSender
{
    Task SendOtpAsync(string toEmail, string otpCode, CancellationToken ct = default);
}
