using Arahk.Neighbor.Web.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Web.Tests;

public class ThaiCopyMasterHousesTests
{
    [Fact]
    public void Master_submenu_labels_are_clean_without_soon()
    {
        ThaiCopy.T("master.page.title").Should().Be("ข้อมูลหลัก");
        ThaiCopy.T("master.select.label").Should().Be("ส่วนข้อมูลหลัก");
        ThaiCopy.T("master.tab.houses").Should().Be("บ้าน");
        ThaiCopy.T("master.tab.calendar").Should().Be("ปฏิทิน");
        ThaiCopy.T("master.tab.activities").Should().Be("กิจกรรม");
        ThaiCopy.T("master.tab.announcements").Should().Be("ข่าวประกาศ");
        ThaiCopy.T("master.tab.users").Should().Be("ผู้ใช้");
        ThaiCopy.T("master.tab.roles").Should().Be("บทบาทผู้ใช้");
        ThaiCopy.T("master.tab.permissions").Should().Be("สิทธิ์");

        foreach (var key in new[]
                 {
                     "master.tab.houses", "master.tab.calendar", "master.tab.activities",
                     "master.tab.announcements", "master.tab.users", "master.tab.roles",
                     "master.tab.permissions"
                 })
        {
            ThaiCopy.T(key).Should().NotContain("เร็วๆ นี้");
        }
    }

    [Fact]
    public void Houses_copy_covers_crud_and_import_guide()
    {
        ThaiCopy.T("houses.action.add").Should().Be("เพิ่มบ้าน");
        ThaiCopy.T("houses.delete.confirm").Should().Be("ลบ");
        ThaiCopy.Format("houses.delete.body", ("houseNo", "12/1"))
            .Should().Be("ต้องการลบบ้านเลขที่ 「12/1」 ออกจากชุมชนหรือไม่?");
        ThaiCopy.T("houses.import.guide.title").Should().Be("วิธีสร้างไฟล์ Excel");
        ThaiCopy.T("houses.error.house_no_duplicate").Should().Be("บ้านเลขที่นี้มีอยู่แล้วในชุมชน");
    }
}
