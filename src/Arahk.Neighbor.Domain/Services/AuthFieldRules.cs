using System.Text.RegularExpressions;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.ValueObjects;

namespace Arahk.Neighbor.Domain.Services;

/// <summary>Pure validation rules shared by client-facing and server-side checks.</summary>
public static class AuthFieldRules
{
    private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);
    private static readonly Regex LetterRegex = new(@"\p{L}", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"\d", RegexOptions.Compiled);

    public static string? ValidateDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return ErrorKeys.DisplayNameRequired;
        var trimmed = displayName.Trim();
        if (trimmed.Length < AuthConstants.DisplayNameMinLength)
            return ErrorKeys.DisplayNameMin;
        if (trimmed.Length > AuthConstants.DisplayNameMaxLength)
            return ErrorKeys.DisplayNameMax;
        return null;
    }

    public static string? ValidateEmail(string? email, bool required = true)
    {
        if (string.IsNullOrWhiteSpace(email))
            return required ? ErrorKeys.EmailRequired : null;
        var trimmed = email.Trim();
        if (trimmed.Length > AuthConstants.EmailMaxLength || !EmailRegex.IsMatch(trimmed))
            return ErrorKeys.EmailFormat;
        return null;
    }

    public static string? ValidatePhone(string? phone, bool required)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return required ? ErrorKeys.PhoneRequired : null;
        return PhoneNumber.TryParse(phone, out _) ? null : ErrorKeys.PhoneFormat;
    }

    public static string? ValidatePasswordForRegister(string? password)
    {
        if (string.IsNullOrEmpty(password))
            return ErrorKeys.PasswordRequired;
        if (password.Length < AuthConstants.PasswordMinLength)
            return ErrorKeys.PasswordMin;
        if (password.Length > AuthConstants.PasswordMaxLength)
            return ErrorKeys.PasswordMin; // silent max — no dedicated copy key
        if (!LetterRegex.IsMatch(password) || !DigitRegex.IsMatch(password))
            return ErrorKeys.PasswordComplexity;
        return null;
    }

    public static string? ValidatePasswordRequired(string? password)
    {
        return string.IsNullOrEmpty(password) ? ErrorKeys.PasswordRequired : null;
    }

    public static string? ValidatePasswordConfirm(string? password, string? confirm)
    {
        if (string.IsNullOrEmpty(confirm))
            return ErrorKeys.PasswordConfirmRequired;
        if (password != confirm)
            return ErrorKeys.PasswordConfirmMismatch;
        return null;
    }

    public static string? ValidateTerms(bool accepted)
    {
        return accepted ? null : ErrorKeys.TermsRequired;
    }

    public static string? ValidateOtpFormat(string? otp)
    {
        if (string.IsNullOrWhiteSpace(otp))
            return ErrorKeys.OtpRequired;
        var trimmed = otp.Trim();
        if (trimmed.Length != AuthConstants.OtpLength || !trimmed.All(char.IsDigit))
            return ErrorKeys.OtpFormat;
        return null;
    }
}
