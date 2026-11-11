using Maviray.Blazor.Components.Core.Models.Tables;

namespace Maviray.Blazor.Components.Material.Tests.Models.Tables;

public class MaviTableCellTests
{
    [Fact]
    public void Guid_IsStableAcrossReads()
    {
        var cell = new MaviTableCell();

        var first = cell.Guid;

        cell.Guid.Should().Be(first);
        System.Guid.TryParse(first, out _).Should().BeTrue();
        new MaviTableCell().Guid.Should().NotBe(first);
    }
}
