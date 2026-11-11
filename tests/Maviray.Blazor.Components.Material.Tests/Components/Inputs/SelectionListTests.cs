namespace Maviray.Blazor.Components.Material.Tests.Components.Inputs;

/// <summary>
///     Selecting options in the dropdown-style inputs.
/// </summary>
public class SelectionListTests : ComponentTestBase
{
    public SelectionListTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Dropdown_ClickingAnOption_SelectsIt_AndClosesTheList()
    {
        string? value = null;
        var cut = Render<MaviDropdown<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple", "banana"]));

        cut.Find("input").Click();
        cut.FindAll("[role=option]").Single(o => o.TextContent.Trim() == "banana").Click();

        value.Should().Be("banana");
        cut.FindAll("[role=listbox]").Should().BeEmpty();
    }

    [Fact]
    public void MultiSelect_ClickingOptions_TogglesThemInTheSelection()
    {
        List<string>? value = [];
        var cut = Render<MaviMultiSelect<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple", "banana", "cherry"]));

        cut.Find("[role=combobox]").Click();
        Option(cut, "apple").Click();
        Option(cut, "cherry").Click();
        Option(cut, "apple").Click();

        value.Should().Equal("cherry");
    }

    private static AngleSharp.Dom.IElement Option(IRenderedComponent<MaviMultiSelect<string, string>> cut, string text) =>
        cut.FindAll("[role=option]").Single(o => o.TextContent.Trim() == text);
}
