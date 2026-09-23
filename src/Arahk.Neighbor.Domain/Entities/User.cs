using Arahk.Neighbor.Domain.ValueObjects;

namespace Arahk.Neighbor.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneE164 { get; private set; }
    public string? PhoneLocal { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public bool EmailVerified { get; private set; }
    public bool TermsAccepted { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { }

    public static User Create(
        string displayName,
        EmailAddress email,
        string passwordHash,
        bool termsAccepted,
        DateTimeOffset createdAt,
        PhoneNumber? phone = null)
    {
        if (!termsAccepted)
            throw new ArgumentException("Terms must be accepted.", nameof(termsAccepted));

        return new User
        {
            Id = Guid.NewGuid(),
            DisplayName = displayName.Trim(),
            Email = email.Value,
            PasswordHash = passwordHash,
            TermsAccepted = true,
            EmailVerified = false,
            CreatedAt = createdAt,
            PhoneE164 = phone?.E164,
            PhoneLocal = phone?.LocalForm
        };
    }

    public void MarkEmailVerified() => EmailVerified = true;

    public void UpdatePendingRegistration(
        string displayName,
        string passwordHash,
        PhoneNumber? phone,
        DateTimeOffset now)
    {
        DisplayName = displayName.Trim();
        PasswordHash = passwordHash;
        PhoneE164 = phone?.E164;
        PhoneLocal = phone?.LocalForm;
        CreatedAt = now;
        EmailVerified = false;
    }
}
