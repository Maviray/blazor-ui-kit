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

    [Fact]
    public void Dropdown_OptionClasses_AreUnchanged()
    {
        string? value = "banana";
        var cut = Render<MaviDropdown<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple", "banana"]));

        cut.Find("input").Click();

        OptionClass(cut.FindAll("[role=option]"), "banana").Should().Be(
            "relative px-3.5 py-2.5 cursor-pointer select-none flex items-center justify-between transition-colors duration-150 " +
            "text-(--theme-primary-nine) bg-black/[0.08] font-medium text-base");
        OptionClass(cut.FindAll("[role=option]"), "apple").Should().Be(
            "relative px-3.5 py-2.5 cursor-pointer select-none flex items-center justify-between transition-colors duration-150 " +
            "text-black/[0.87] hover:bg-black/[0.04] active:bg-black/[0.08] text-base");
    }

    [Fact]
    public void MultiSelect_OptionClasses_AreUnchanged()
    {
        List<string>? value = ["banana"];
        var cut = Render<MaviMultiSelect<string, string>>(p => p
            .Bind(x => x.Value, value, v => value = v)
            .Add(x => x.Items, ["apple", "banana"]));

        cut.Find("[role=combobox]").Click();

        OptionClass(cut.FindAll("[role=option]"), "banana").Should().Be(
            "relative px-3.5 py-2.5 cursor-pointer select-none flex items-center gap-3 transition-colors duration-150 " +
            "text-(--theme-primary-nine) bg-black/[0.04] text-base");
        OptionClass(cut.FindAll("[role=option]"), "apple").Should().Be(
            "relative px-3.5 py-2.5 cursor-pointer select-none flex items-center gap-3 transition-colors duration-150 " +
            "text-black/[0.87] hover:bg-black/[0.04] active:bg-black/[0.08] text-base");
    }

    private static string? OptionClass(IReadOnlyList<AngleSharp.Dom.IElement> options, string text) =>
        options.Single(o => o.TextContent.Trim() == text).GetAttribute("class");

    private static AngleSharp.Dom.IElement Option(IRenderedComponent<MaviMultiSelect<string, string>> cut, string text) =>
        cut.FindAll("[role=option]").Single(o => o.TextContent.Trim() == text);
}
