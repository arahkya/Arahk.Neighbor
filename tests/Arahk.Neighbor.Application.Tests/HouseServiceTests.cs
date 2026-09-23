using Arahk.Neighbor.Application.Common;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Application.Models;
using Arahk.Neighbor.Application.Services;
using Arahk.Neighbor.Domain.Constants;
using Arahk.Neighbor.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Arahk.Neighbor.Application.Tests;

public class HouseServiceTests
{
    private readonly Guid _community = HouseConstants.DefaultCommunityId;
    private readonly Mock<IHouseRepository> _repo = new();
    private readonly Mock<IHouseExcelParser> _excel = new();
    private readonly Mock<IClock> _clock = new();
    private readonly HouseService _sut;
    private readonly DateTimeOffset _now = DateTimeOffset.Parse("2026-09-23T10:00:00Z");

    public HouseServiceTests()
    {
        _clock.Setup(c => c.UtcNow).Returns(_now);
        _sut = new HouseService(_repo.Object, _excel.Object, _clock.Object);
    }

    [Fact]
    public async Task Create_rejects_empty_house_no()
    {
        var result = await _sut.CreateAsync(_community, new CreateHouseRequest("  ", null));
        result.Succeeded.Should().BeFalse();
        result.FieldErrors["houseNo"].Should().Be(ErrorKeys.HouseNoRequired);
    }

    [Fact]
    public async Task Create_rejects_duplicate()
    {
        var existing = House.Create(_community, "12/1", null, _now);
        _repo.Setup(r => r.FindByHouseNoAsync(_community, "12/1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _sut.CreateAsync(_community, new CreateHouseRequest(" 12/1 ", "ซอย"));
        result.Succeeded.Should().BeFalse();
        result.FieldErrors["houseNo"].Should().Be(ErrorKeys.HouseNoDuplicate);
    }

    [Fact]
    public async Task Create_succeeds_and_trims()
    {
        _repo.Setup(r => r.FindByHouseNoAsync(_community, "12/1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((House?)null);
        House? added = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<House>(), It.IsAny<CancellationToken>()))
            .Callback<House, CancellationToken>((h, _) => added = h)
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(_community, new CreateHouseRequest(" 12/1 ", " ซอย "));
        result.Succeeded.Should().BeTrue();
        result.Data!.HouseNo.Should().Be("12/1");
        result.Data.Soi.Should().Be("ซอย");
        added.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_excludes_self_on_uniqueness()
    {
        var self = House.Create(_community, "12/1", null, _now);
        _repo.Setup(r => r.GetByIdAsync(_community, self.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(self);
        _repo.Setup(r => r.FindByHouseNoAsync(_community, "12/1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(self);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<House>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.UpdateAsync(_community, new UpdateHouseRequest(self.Id, "12/1", "ใหม่"));
        result.Succeeded.Should().BeTrue();
        result.Data!.Soi.Should().Be("ใหม่");
    }

    [Fact]
    public async Task Update_rejects_duplicate_of_other()
    {
        var self = House.Create(_community, "12/1", null, _now);
        var other = House.Create(_community, "12/2", null, _now);
        _repo.Setup(r => r.GetByIdAsync(_community, self.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(self);
        _repo.Setup(r => r.FindByHouseNoAsync(_community, "12/2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(other);

        var result = await _sut.UpdateAsync(_community, new UpdateHouseRequest(self.Id, "12/2", null));
        result.Succeeded.Should().BeFalse();
        result.FieldErrors["houseNo"].Should().Be(ErrorKeys.HouseNoDuplicate);
    }

    [Fact]
    public async Task Delete_succeeds()
    {
        var house = House.Create(_community, "15", null, _now);
        _repo.Setup(r => r.GetByIdAsync(_community, house.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(house);
        _repo.Setup(r => r.DeleteAsync(house, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(_community, house.Id);
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task Import_insert_only_skips_duplicates()
    {
        var existing = House.Create(_community, "12/1", null, _now);
        _repo.Setup(r => r.ListByCommunityAsync(_community, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<House> { existing });
        _excel.Setup(e => e.Parse(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>()))
            .Returns(Result<IReadOnlyList<HouseImportRow>>.Ok(new List<HouseImportRow>
            {
                new(2, "12/1", "เก่า"),
                new(3, "12/2", null),
                new(4, "12/2", "ซ้ำในไฟล์"),
                new(5, null, "ไม่มีเลข"),
                new(6, "15", "ซอย")
            }));
        _repo.Setup(r => r.AddAsync(It.IsAny<House>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await using var stream = new MemoryStream();
        var result = await _sut.ImportAsync(_community, stream, "houses.xlsx", 100);
        result.Succeeded.Should().BeTrue();
        result.Data!.Ok.Should().Be(2); // 12/2 and 15
        result.Data.Skip.Should().Be(2); // existing + file dup
        result.Data.Fail.Should().Be(1); // missing no
    }

    [Fact]
    public async Task Import_file_rejected_propagates()
    {
        _excel.Setup(e => e.Parse(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>()))
            .Returns(Result<IReadOnlyList<HouseImportRow>>.Fail(ErrorKeys.HouseImportNoSheet));

        await using var stream = new MemoryStream();
        var result = await _sut.ImportAsync(_community, stream, "x.xlsx", 10);
        result.Data!.FileRejected.Should().BeTrue();
        result.Data.FileErrorKey.Should().Be(ErrorKeys.HouseImportNoSheet);
    }
}
