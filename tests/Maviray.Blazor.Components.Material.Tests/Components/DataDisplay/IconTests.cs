using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Material.Components.DataDisplay.Icons;

namespace Maviray.Blazor.Components.Material.Tests.Components.DataDisplay;

public class IconTests : ComponentTestBase
{
    public static TheoryData<IconMiniType> MiniIcons => new(Enum.GetValues<IconMiniType>());
    public static TheoryData<IconOnlineType> OnlineIcons => new(Enum.GetValues<IconOnlineType>());
    public static TheoryData<IconSolidType> SolidIcons => new(Enum.GetValues<IconSolidType>());

    [Theory]
    [MemberData(nameof(MiniIcons))]
    public void EveryMiniIcon_RendersPaths(IconMiniType icon)
    {
        var cut = Render<MaviIconMini>(p => p.Add(x => x.IconMiniType, icon));

        cut.FindAll("path").Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(OnlineIcons))]
    public void EveryOnlineIcon_RendersPaths(IconOnlineType icon)
    {
        var cut = Render<MaviIconOnline>(p => p.Add(x => x.IconOnlineType, icon));

        cut.FindAll("path").Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(SolidIcons))]
    public void EverySolidIcon_RendersPaths(IconSolidType icon)
    {
        var cut = Render<MaviIconSolid>(p => p.Add(x => x.IconSolidType, icon));

        cut.FindAll("path").Should().NotBeEmpty();
    }

    [Fact]
    public void IconPaths_AreSharedBetweenLookups()
    {
        ReferenceEquals(
                MaviIconMiniPaths.GetIconPaths(IconMiniType.AcademicCapMini),
                MaviIconMiniPaths.GetIconPaths(IconMiniType.AcademicCapMini))
            .Should().BeTrue("a lookup must not build a new path list on every render");
    }
}
