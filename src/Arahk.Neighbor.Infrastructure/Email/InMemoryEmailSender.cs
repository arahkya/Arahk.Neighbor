using System.Collections.Concurrent;
using Arahk.Neighbor.Application.Interfaces;

namespace Arahk.Neighbor.Infrastructure.Email;

public record SentEmail(string To, string Subject, string Body, DateTimeOffset SentAt);

/// <summary>Dev sink — stores OTP emails in memory for local demo / tests.</summary>
public class InMemoryEmailSender : IEmailSender
{
    private readonly ConcurrentQueue<SentEmail> _sent = new();

    public IReadOnlyCollection<SentEmail> Sent => _sent.ToArray();

    public string? LastOtpCode { get; private set; }
    public string? LastOtpEmail { get; private set; }

    public Task SendOtpAsync(string toEmail, string otpCode, CancellationToken ct = default)
    {
        LastOtpCode = otpCode;
        LastOtpEmail = toEmail;
        _sent.Enqueue(new SentEmail(
            toEmail,
            "Neighbor OTP",
            $"รหัส OTP ของคุณคือ {otpCode} (หมดอายุใน 10 นาที)",
            DateTimeOffset.UtcNow));
        return Task.CompletedTask;
    }
}
