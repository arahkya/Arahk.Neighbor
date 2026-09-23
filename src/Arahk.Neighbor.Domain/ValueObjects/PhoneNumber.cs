using System.Text.RegularExpressions;
using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Domain.ValueObjects;

/// <summary>
/// Thai mobile phone: after normalize must match ^0[689]\d{8}$
/// Accepts +66 / 66 prefixes and strips spaces, dashes, parentheses.
/// Stored as E.164 (+66XXXXXXXXX).
/// </summary>
public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex LocalRegex = new(@"^0[689]\d{8}$", RegexOptions.Compiled);

    public string LocalForm { get; } // 0XXXXXXXXX
    public string E164 { get; }      // +66XXXXXXXXX

    private PhoneNumber(string localForm)
    {
        LocalForm = localForm;
        E164 = "+66" + localForm[1..];
    }

    public static bool TryParse(string? input, out PhoneNumber? phone)
    {
        phone = null;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var normalized = Normalize(input);
        if (!LocalRegex.IsMatch(normalized))
            return false;

        phone = new PhoneNumber(normalized);
        return true;
    }

    public static string Normalize(string input)
    {
        var cleaned = input.Trim();
        cleaned = cleaned.Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        if (cleaned.StartsWith("+66", StringComparison.Ordinal))
            cleaned = "0" + cleaned[3..];
        else if (cleaned.StartsWith("66", StringComparison.Ordinal) && cleaned.Length == 11)
            cleaned = "0" + cleaned[2..];

        return cleaned;
    }

    public bool Equals(PhoneNumber? other) =>
        other is not null && LocalForm == other.LocalForm;

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);
    public override int GetHashCode() => LocalForm.GetHashCode(StringComparison.Ordinal);
    public override string ToString() => LocalForm;
}
