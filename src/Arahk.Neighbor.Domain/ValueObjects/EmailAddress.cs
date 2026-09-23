using System.Text.RegularExpressions;
using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Domain.ValueObjects;

public sealed class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex FormatRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static bool TryParse(string? input, out EmailAddress? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var trimmed = input.Trim();
        if (trimmed.Length > AuthConstants.EmailMaxLength)
            return false;
        if (!FormatRegex.IsMatch(trimmed))
            return false;

        email = new EmailAddress(trimmed.ToLowerInvariant());
        return true;
    }

    public bool Equals(EmailAddress? other) =>
        other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as EmailAddress);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
    public override string ToString() => Value;

    /// <summary>Mask for OTP screen e.g. yo***@email.com</summary>
    public string ToMasked()
    {
        var at = Value.IndexOf('@');
        if (at <= 0) return Value;
        var local = Value[..at];
        var domain = Value[(at + 1)..];
        var visible = Math.Min(2, local.Length);
        return local[..visible] + "***@" + domain;
    }
}
