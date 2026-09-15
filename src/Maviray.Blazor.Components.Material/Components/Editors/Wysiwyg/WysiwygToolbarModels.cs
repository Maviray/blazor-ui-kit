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

public static class WysiwygDefaults
{
    public static readonly IReadOnlyList<string> Fonts =
    [
        "Arial", "Helvetica", "Georgia", "Tahoma", "Times New Roman", "Verdana", "Courier New",
    ];

    public static readonly IReadOnlyList<string> Colors =
    [
        "#000000", "#495057", "#e03131", "#d6336c", "#ae3ec9", "#7048e8", "#1971c2",
        "#0c8599", "#2f9e44", "#f08c00", "#e8590c", "#ffffff",
    ];

    /// <summary>Font sizes (in px) offered by the size dropdown; 16 is the editor base.</summary>
    public static readonly IReadOnlyList<int> Sizes =
    [
        10, 12, 14, 16, 18, 24, 32,
    ];
}
