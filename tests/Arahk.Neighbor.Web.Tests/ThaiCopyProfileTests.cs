using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Web.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Web.Tests;

public class ThaiCopyProfileTests
{
    [Fact]
    public void Profile_menu_and_titles_are_locked_thai()
    {
        ThaiCopy.T("profile.menu.my_profile").Should().Be("โปรไฟล์ของฉัน");
        ThaiCopy.T("profile.menu.change_password").Should().Be("เปลี่ยนรหัสผ่าน");
        ThaiCopy.T("profile.menu.sign_out").Should().Be("ออกจากระบบ");
        ThaiCopy.T("profile.title").Should().Be("โปรไฟล์ของฉัน");
        ThaiCopy.T("password_change.title").Should().Be("เปลี่ยนรหัสผ่าน");
        ThaiCopy.T("profile.toast.saved").Should().Be("บันทึกโปรไฟล์แล้ว");
        ThaiCopy.T("password_change.toast.saved").Should().Be("เปลี่ยนรหัสผ่านแล้ว");
        ThaiCopy.T("nav.section").Should().Be("เมนูหลัก");
    }

    [Fact]
    public void Profile_error_keys_are_thai()
    {
        ThaiCopy.T(ErrorKeys.PasswordCurrentRequired).Should().Be("กรุณากรอกรหัสผ่านปัจจุบัน");
        ThaiCopy.T(ErrorKeys.PasswordCurrentInvalid).Should().Be("รหัสผ่านปัจจุบันไม่ถูกต้อง");
        ThaiCopy.T(ErrorKeys.PasswordNewRequired).Should().Be("กรุณากรอกรหัสผ่านใหม่");
        ThaiCopy.T(ErrorKeys.DisplayNameMin).Should().Be("ชื่อสั้นเกินไป (อย่างน้อย 2 ตัวอักษร)");
        ThaiCopy.T(ErrorKeys.PhoneFormat).Should().Be("รูปแบบเบอร์โทรไม่ถูกต้อง");
    }

    [Fact]
    public void Welcome_named_formats_display_name()
    {
        ThaiCopy.Format("home.welcome.headline_named", ("displayName", "อารักษ์"))
            .Should().Be("ยินดีต้อนรับกลับบ้าน, อารักษ์");
    }
}
