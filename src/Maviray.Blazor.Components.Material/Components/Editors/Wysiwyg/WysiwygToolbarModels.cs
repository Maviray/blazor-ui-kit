namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

public sealed record WysiwygBlockStyle(string Label, string Tag)
{
    public static readonly IReadOnlyList<WysiwygBlockStyle> All =
    [
        new("Normal", "p"),
        new("Heading 1", "h1"),
        new("Heading 2", "h2"),
        new("Heading 3", "h3"),
        new("Heading 4", "h4"),
        new("Heading 5", "h5"),
        new("Heading 6", "h6"),
        new("Quote", "blockquote"),
        new("Code", "pre"),
    ];
}
