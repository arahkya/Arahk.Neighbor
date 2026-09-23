namespace Arahk.Neighbor.Domain.Constants;

/// <summary>Error / copy keys aligned with UX 03-copy-th.md</summary>
public static class ErrorKeys
{
    public const string EmailRequired = "err.email.required";
    public const string EmailFormat = "err.email.format";
    public const string EmailDuplicate = "err.email.duplicate";
    public const string PhoneRequired = "err.phone.required";
    public const string PhoneFormat = "err.phone.format";
    public const string PasswordRequired = "err.password.required";
    public const string PasswordMin = "err.password.min";
    public const string PasswordComplexity = "err.password.complexity";
    public const string PasswordConfirmRequired = "err.password_confirm.required";
    public const string PasswordConfirmMismatch = "err.password_confirm.mismatch";
    public const string PasswordCurrentRequired = "err.password.current_required";
    public const string PasswordCurrentInvalid = "err.password.current_invalid";
    public const string PasswordNewRequired = "err.password.new_required";
    public const string DisplayNameRequired = "err.displayname.required";
    public const string DisplayNameMin = "err.displayname.min";
    public const string DisplayNameMax = "err.displayname.max";
    public const string TermsRequired = "err.terms.required";
    public const string LoginCredentials = "err.login.credentials";
    public const string LoginCredentialsPhone = "err.login.credentials_phone";
    public const string LoginPhoneNotLinked = "err.login.phone_not_linked";
    public const string OtpRequired = "err.otp.required";
    public const string OtpFormat = "err.otp.format";
    public const string OtpInvalid = "err.otp.invalid";
    public const string OtpInvalidRemaining = "err.otp.invalid_remaining";
    public const string OtpExpired = "err.otp.expired";
    public const string OtpMaxAttempts = "err.otp.max_attempts";
    public const string OtpCooldown = "err.otp.cooldown";
    public const string NetworkGeneric = "error.network_generic";
    public const string ServerGeneric = "error.server_generic";

    // Houses (Master Data) — keys align with UX 03-copy-th.md
    public const string HouseNoRequired = "houses.error.house_no_required";
    public const string HouseNoDuplicate = "houses.error.house_no_duplicate";
    public const string HouseNoTooLong = "houses.error.house_no_too_long";
    public const string SoiTooLong = "houses.error.soi_too_long";
    public const string HouseNetwork = "houses.error.network";
    public const string HouseServer = "houses.error.server";
    public const string HouseDeleteFailed = "houses.error.delete_failed";
    public const string HouseDeleteConflict = "houses.error.delete_conflict";
    public const string HouseNotFound = "houses.error.not_found";

    public const string HouseImportBadType = "houses.import.error.bad_type";
    public const string HouseImportEmptyFile = "houses.import.error.empty_file";
    public const string HouseImportNoSheet = "houses.import.error.no_sheet";
    public const string HouseImportBadHeaders = "houses.import.error.bad_headers";
    public const string HouseImportMissingColumns = "houses.import.error.missing_columns";
    public const string HouseImportTooLarge = "houses.import.error.too_large";
    public const string HouseImportTooManyRows = "houses.import.error.too_many_rows";
    public const string HouseImportGeneric = "houses.import.error.generic";
    public const string HouseImportNetwork = "houses.import.error.network";
}
