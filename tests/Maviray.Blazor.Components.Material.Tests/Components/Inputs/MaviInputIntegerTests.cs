namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     Core functionality tests for MaviInputInteger component.
///     This partial class focuses on basic rendering, parameter binding, and core behaviors.
/// </summary>
public class MaviInputIntegerTests : ComponentTestBase
{
    [Fact]
    public void Renders_NumberInput_WithBoundValue()
    {
        var value = 42;

        var cut = Render<MaviInputInteger>(parameters => parameters
            .Bind(p => p.Value, value, newValue => value = newValue));

        var input = cut.Find("input");

        input.GetAttribute("type").Should().Be("number");
        input.GetAttribute("value").Should().Be("42");
    }
}
