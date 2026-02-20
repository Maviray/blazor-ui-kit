using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Toastr;

public class ToastItem
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Message { get; init; } = string.Empty;

    public string? Title { get; init; }

    public ThemeColorScheme ColorScheme { get; init; } = ThemeColorScheme.Default;

    public ElementVariant Variant { get; init; } = ElementVariant.Filled;

    public ElementSize Size { get; init; } = ElementSize.Regular;

    public ToastrPosition Position { get; init; } = ToastrPosition.CenterTop;

    /// <summary>
    /// Duration in milliseconds. 0 or negative means sticky (stays until manually dismissed).
    /// </summary>
    public int Duration { get; init; } = 3000;

    public bool IsSticky => Duration <= 0;

    public string? Icon { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
