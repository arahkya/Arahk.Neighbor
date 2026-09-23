using Arahk.Neighbor.Web.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Web.Tests;

public class ThaiCopyRolesPermissionsTests
{
    [Fact]
    public void Permissions_catalog_copy_is_master_toggle_not_add_delete()
    {
        ThaiCopy.T("perms.page.title").Should().Be("สิทธิ์ในระบบ");
        ThaiCopy.T("perms.col.master").Should().Be("เปิดในระบบ");
        ThaiCopy.T("perms.callout.static").Should().Contain("ไม่เพิ่มหรือลบ");
        ThaiCopy.T("perms.disable.confirm").Should().Be("ปิดในระบบ");
        ThaiCopy.T("roles.perm.locked_badge").Should().Contain("ปิดในระบบ");
    }

    [Fact]
    public void Roles_master_detail_copy_and_five_role_labels()
    {
        ThaiCopy.T("roles.page.subtitle").Should().Contain("เลือกบทบาท");
        ThaiCopy.T("roles.action.save").Should().Be("บันทึก");
        ThaiCopy.T("roles.action.cancel").Should().Be("ยกเลิก");
        ThaiCopy.T("role.resident").Should().Be("ลูกบ้าน");
        ThaiCopy.T("role.juristic").Should().Be("นิติบุคคล");
        ThaiCopy.T("role.committee").Should().Be("คณะกรรมการหมู่บ้าน");
        ThaiCopy.T("role.auditor").Should().Be("ผู้ตรวจสอบบัญชีภายนอก");
        ThaiCopy.T("role.security").Should().Be("เจ้าหน้าที่รปภ.");
    }
}
