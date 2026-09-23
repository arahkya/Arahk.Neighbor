using Arahk.Neighbor.Web.Services;
using FluentAssertions;

namespace Arahk.Neighbor.Web.Tests;

public class ThaiCopyHomeTests
{
    [Fact]
    public void Welcome_copy_matches_locked_ux()
    {
        ThaiCopy.T("home.welcome.headline").Should().Be("ยินดีต้อนรับกลับบ้าน");
        ThaiCopy.T("home.welcome.body")
            .Should().Be("Neighbor ช่วยรวบรวมและจัดการข้อมูลของหมู่บ้าน เพื่อรองรับความต้องการของลูกบ้านและนิติบุคคลผู้บริหารดูแล — จุดเด่นคือข้อมูลเป็นของลูกบ้านและหมู่บ้าน แม้เปลี่ยนบริษัทนิติบุคคลที่ดูแล ข้อมูลก็ยังอยู่กับหมู่บ้านเช่นเดิม");
        ThaiCopy.T("home.welcome.tip")
            .Should().Be("ใช้เมนูด้านข้างเพื่อไปยังบริการต่างๆ ของหมู่บ้าน");
    }

    [Fact]
    public void Nav_labels_are_locked_thai()
    {
        ThaiCopy.T("nav.home").Should().Be("หน้าหลัก");
        ThaiCopy.T("nav.payment").Should().Be("การชำระเงิน");
        ThaiCopy.T("nav.packages").Should().Be("แพ็กเกจ");
        ThaiCopy.T("nav.visitor").Should().Be("ผู้มาเยือน");
        ThaiCopy.T("nav.master_data").Should().Be("ข้อมูลหลัก");
    }

    [Fact]
    public void Soft_soon_body_named_formats_nav_label()
    {
        ThaiCopy.Format("soon.body_named", ("navLabel", "การชำระเงิน"))
            .Should().Be("เมนู「การชำระเงิน」ยังไม่พร้อมใช้งานในตอนนี้");
    }
}
