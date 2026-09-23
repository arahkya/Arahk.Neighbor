using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Web.Services;

/// <summary>Thai UI strings locked from UX 03-copy-th.md.</summary>
public static class ThaiCopy
{
    private static readonly Dictionary<string, string> Map = new()
    {
        ["app.wordmark"] = "Neighbor",
        ["app.tagline"] = "เพื่อนบ้าน",
        [ErrorKeys.NetworkGeneric] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง",
        [ErrorKeys.ServerGeneric] = "ระบบขัดข้องชั่วคราว กรุณาลองใหม่ภายหลัง",
        ["action.retry"] = "ลองใหม่",

        ["login.title"] = "เข้าสู่ระบบ",
        ["login.mode.email"] = "อีเมล",
        ["login.mode.phone"] = "เบอร์โทร",
        ["login.cta"] = "เข้าสู่ระบบ",
        ["login.link.register"] = "ยังไม่มีบัญชี? สมัครสมาชิก",
        ["login.link.forgot"] = "ลืมรหัสผ่าน?",
        ["login.forgot.soon"] = "ฟีเจอร์นี้จะเปิดให้ใช้เร็วๆ นี้",
        ["login.email.label"] = "อีเมล",
        ["login.email.placeholder"] = "you@email.com",
        ["login.phone.label"] = "เบอร์โทรศัพท์",
        ["login.phone.placeholder"] = "08X XXX XXXX",
        ["login.phone.helper"] = "ใช้เบอร์ที่ผูกกับบัญชีแล้ว",
        ["login.password.label"] = "รหัสผ่าน",
        ["login.password.placeholder"] = "รหัสผ่านของคุณ",

        [ErrorKeys.EmailRequired] = "กรุณากรอกอีเมล",
        [ErrorKeys.EmailFormat] = "รูปแบบอีเมลไม่ถูกต้อง",
        [ErrorKeys.PhoneRequired] = "กรุณากรอกเบอร์โทรศัพท์",
        [ErrorKeys.PhoneFormat] = "รูปแบบเบอร์โทรไม่ถูกต้อง",
        [ErrorKeys.PasswordRequired] = "กรุณากรอกรหัสผ่าน",
        [ErrorKeys.LoginCredentials] = "อีเมลหรือรหัสผ่านไม่ถูกต้อง",
        [ErrorKeys.LoginCredentialsPhone] = "เบอร์โทรหรือรหัสผ่านไม่ถูกต้อง",
        [ErrorKeys.LoginPhoneNotLinked] = "ไม่พบบัญชีที่ผูกกับเบอร์นี้",

        ["register.title"] = "สมัครสมาชิก",
        ["register.cta"] = "สมัครสมาชิก",
        ["register.link.login"] = "มีบัญชีแล้ว? เข้าสู่ระบบ",
        ["register.terms.prefix"] = "ฉันยอมรับ",
        ["register.terms.tos"] = "ข้อกำหนดการใช้บริการ",
        ["register.terms.and"] = "และ",
        ["register.terms.privacy"] = "นโยบายความเป็นส่วนตัว",
        ["register.success.otp_sent"] = "สมัครสำเร็จ เราส่งรหัส OTP ไปที่อีเมลของคุณแล้ว",
        ["register.displayname.label"] = "ชื่อที่แสดง",
        ["register.displayname.placeholder"] = "เช่น คุณมานี",
        ["register.displayname.helper"] = "เพื่อนบ้านจะเห็นชื่อนี้",
        ["register.email.label"] = "อีเมล",
        ["register.email.placeholder"] = "you@email.com",
        ["register.email.helper"] = "เราจะส่งรหัสยืนยันไปที่อีเมลนี้",
        ["register.phone.label"] = "เบอร์โทรศัพท์ (ไม่บังคับ)",
        ["register.phone.placeholder"] = "08X XXX XXXX",
        ["register.phone.helper"] = "เพิ่มทีหลังได้ · ใช้เข้าสู่ระบบด้วยเบอร์ได้เมื่อผูกแล้ว",
        ["register.password.label"] = "รหัสผ่าน",
        ["register.password.placeholder"] = "อย่างน้อย 8 ตัวอักษร",
        ["register.password.helper"] = "ต้องมีทั้งตัวอักษรและตัวเลข",
        ["register.password_confirm.label"] = "ยืนยันรหัสผ่าน",
        ["register.password_confirm.placeholder"] = "กรอกรหัสผ่านอีกครั้ง",

        [ErrorKeys.DisplayNameRequired] = "กรุณากรอกชื่อที่แสดง",
        [ErrorKeys.DisplayNameMin] = "ชื่อสั้นเกินไป (อย่างน้อย 2 ตัวอักษร)",
        [ErrorKeys.DisplayNameMax] = "ชื่อยาวเกินไป (ไม่เกิน 50 ตัวอักษร)",
        [ErrorKeys.EmailDuplicate] = "อีเมลนี้ถูกใช้แล้ว",
        [ErrorKeys.PasswordMin] = "รหัสผ่านต้องมีอย่างน้อย 8 ตัวอักษร",
        [ErrorKeys.PasswordComplexity] = "รหัสผ่านต้องมีทั้งตัวอักษรและตัวเลข",
        [ErrorKeys.PasswordConfirmRequired] = "กรุณายืนยันรหัสผ่าน",
        [ErrorKeys.PasswordConfirmMismatch] = "รหัสผ่านไม่ตรงกัน",
        [ErrorKeys.TermsRequired] = "กรุณายอมรับข้อกำหนดเพื่อสมัครสมาชิก",

        ["otp.title"] = "ยืนยันอีเมล",
        ["otp.subtitle"] = "กรอกรหัส 6 หลักที่ส่งไปยัง",
        ["otp.field_label"] = "รหัส OTP",
        ["otp.placeholder"] = "000000",
        ["otp.helper_expiry"] = "รหัสมีอายุ 10 นาที",
        ["otp.cta"] = "ยืนยัน",
        ["otp.resend"] = "ส่งรหัสอีกครั้ง",
        ["otp.resend_in"] = "ส่งอีกครั้งใน {seconds} วินาที",
        ["otp.back"] = "กลับไปแก้ไขข้อมูลสมัคร",
        ["otp.toast.sent"] = "ส่งรหัส OTP แล้ว",
        ["otp.toast.resent"] = "ส่งรหัส OTP ใหม่แล้ว",
        ["otp.success"] = "ยืนยันอีเมลสำเร็จ",

        [ErrorKeys.OtpRequired] = "กรุณากรอกรหัส 6 หลัก",
        [ErrorKeys.OtpFormat] = "รหัส OTP ต้องเป็นตัวเลข 6 หลัก",
        [ErrorKeys.OtpInvalid] = "รหัสไม่ถูกต้อง กรุณาลองใหม่",
        [ErrorKeys.OtpInvalidRemaining] = "รหัสไม่ถูกต้อง (เหลือ {n} ครั้ง)",
        [ErrorKeys.OtpExpired] = "รหัสหมดอายุแล้ว กรุณาขอรหัสใหม่",
        [ErrorKeys.OtpMaxAttempts] = "คุณกรอกรหัสผิดเกินกำหนด กรุณาขอรหัสใหม่",
        [ErrorKeys.OtpCooldown] = "กรุณารอสักครู่ก่อนขอรหัสใหม่",

        ["state.loading"] = "กำลังโหลด...",
        ["state.submitting"] = "กำลังดำเนินการ...",
        ["state.sending_otp"] = "กำลังส่งรหัส...",
        ["home.welcome"] = "ยินดีต้อนรับ",
        ["home.signed_in_as"] = "เข้าสู่ระบบแล้วในชื่อ",
        ["home.logout"] = "ออกจากระบบ",
        ["dev.otp_hint"] = "รหัส OTP (โหมดพัฒนา)",
    };

    public static string T(string key) => Map.TryGetValue(key, out var v) ? v : key;

    public static string Format(string key, params (string name, object? value)[] args)
    {
        var text = T(key);
        foreach (var (name, value) in args)
            text = text.Replace("{" + name + "}", value?.ToString() ?? "", StringComparison.Ordinal);
        return text;
    }
}
