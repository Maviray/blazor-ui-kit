namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

internal static class WysiwygIcons
{
    private const string Open =
        "<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">";
    private const string Close = "</svg>";

    public static readonly string Refresh = Open +
        "<polyline points=\"23 4 23 10 17 10\"/><polyline points=\"1 20 1 14 7 14\"/>" +
        "<path d=\"M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15\"/>" + Close;

    public static readonly string Save = Open +
        "<path d=\"M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z\"/>" +
        "<polyline points=\"17 21 17 13 7 13 7 21\"/><polyline points=\"7 3 7 8 15 8\"/>" + Close;

    public static readonly string Bold = Open +
        "<path d=\"M6 4h8a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/><path d=\"M6 12h9a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/>" + Close;

    public static readonly string Italic = Open +
        "<line x1=\"19\" y1=\"4\" x2=\"10\" y2=\"4\"/><line x1=\"14\" y1=\"20\" x2=\"5\" y2=\"20\"/>" +
        "<line x1=\"15\" y1=\"4\" x2=\"9\" y2=\"20\"/>" + Close;

    public static readonly string Underline = Open +
        "<path d=\"M6 3v7a6 6 0 0 0 6 6 6 6 0 0 0 6-6V3\"/><line x1=\"4\" y1=\"21\" x2=\"20\" y2=\"21\"/>" + Close;

    public static readonly string Strikethrough = Open +
        "<line x1=\"4\" y1=\"12\" x2=\"20\" y2=\"12\"/><path d=\"M6 7a4 3 0 0 1 8 0M10 12a4 3 0 0 1 4 3 4 3 0 0 1-8 0\"/>" + Close;

    public static readonly string Eraser = Open +
        "<path d=\"M20 20H7L3 16a2 2 0 0 1 0-3l9-9a2 2 0 0 1 3 0l5 5a2 2 0 0 1 0 3l-7 7\"/>" +
        "<line x1=\"18\" y1=\"12.5\" x2=\"9.5\" y2=\"4\"/>" + Close;

    public static readonly string Paragraph = Open +
        "<path d=\"M13 4v16M17 4v16M17 4H9a5 5 0 0 0 0 10h4\"/>" + Close;
}
