namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     Rendering tests for MaviButtonIcon component.
/// </summary>
public class MaviButtonIconTests : ComponentTestBase
{
    [Fact]
    public void Renders_Icon_AsCenteredFlexContainer()
    {
        // Icon fonts such as Lineicons set their font only on the ::before glyph, so a plain <i> would
        // place the glyph on the button text font's baseline, visibly above the circle's center.
        var cut = Render<MaviButtonIcon>(parameters => parameters
            .Add(p => p.Icon, "lni lni-alarm-1"));

        var icon = cut.Find("button > i");

        icon.ClassList.Should().Contain(new[] { "lni", "lni-alarm-1", "inline-flex", "items-center", "justify-center" });
    }
}
