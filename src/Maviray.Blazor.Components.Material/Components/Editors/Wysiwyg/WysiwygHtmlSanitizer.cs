using Ganss.Xss;

namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

/// <summary>
///     The editor's default HTML cleaning policy: HtmlSanitizer's defaults, adjusted to keep everything the
///     toolbar produces and to drop form controls. Register your own <see cref="IHtmlSanitizer" /> in DI to
///     replace it.
/// </summary>
internal static class WysiwygHtmlSanitizer
{
    // The editor never produces these. In stored or pasted content they could pose as a login prompt.
    private static readonly string[] FormControlTags =
        ["form", "input", "button", "select", "option", "optgroup", "textarea", "datalist", "keygen"];

    /// <summary>Shared instance; <see cref="HtmlSanitizer.Sanitize" /> is thread-safe once configured.</summary>
    public static IHtmlSanitizer Default { get; } = Create();

    private static HtmlSanitizer Create()
    {
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedTags.ExceptWith(FormControlTags);

        // <font face="..."> comes from the font picker.
        sanitizer.AllowedAttributes.Add("face");

        sanitizer.AllowedSchemes.Add("mailto");
        sanitizer.AllowedSchemes.Add("tel");

        sanitizer.FilterUrl += AllowEmbeddedImages;

        return sanitizer;
    }

    // Pictures inserted through the toolbar are embedded as data URLs. Allow those on <img> only:
    // an image cannot run script, while a data URL in a link could open an attacker's page.
    private static void AllowEmbeddedImages(object? sender, FilterUrlEventArgs e)
    {
        if (e.SanitizedUrl is null
            && string.Equals(e.Tag.NodeName, "IMG", StringComparison.OrdinalIgnoreCase)
            && e.OriginalUrl.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
        {
            e.SanitizedUrl = e.OriginalUrl;
        }
    }
}
