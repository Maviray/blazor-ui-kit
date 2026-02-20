using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Options;

public class MaviToastrOptions
{
    /// <summary>
    /// Default duration in milliseconds for timed toasts. 0 means sticky.
    /// </summary>
    public int DefaultDuration { get; set; } = 3000;

    /// <summary>
    /// Maximum number of simultaneously visible toasts. Null means unlimited.
    /// </summary>
    public int? MaxVisibleCount { get; set; }

    /// <summary>
    /// Default position for new toasts.
    /// </summary>
    public ToastrPosition DefaultPosition { get; set; } = ToastrPosition.CenterTop;

    /// <summary>
    /// Default color scheme for new toasts.
    /// </summary>
    public ThemeColorScheme DefaultColorScheme { get; set; } = ThemeColorScheme.Default;

    /// <summary>
    /// Default visual variant for new toasts.
    /// </summary>
    public ElementVariant DefaultVariant { get; set; } = ElementVariant.Filled;

    /// <summary>
    /// Default size for new toasts.
    /// </summary>
    public ElementSize DefaultSize { get; set; } = ElementSize.Regular;
}
