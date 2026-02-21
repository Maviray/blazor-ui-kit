using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Material.Components.DataDisplay;

public sealed record MaviToastNotification(
    string Id,
    string? Title,
    string? Message,
    ThemeColorScheme ThemeColorScheme = ThemeColorScheme.Default,
    ElementVariant ElementVariant = ElementVariant.Filled,
    ElementSize ElementSize = ElementSize.Regular,
    DateTimeOffset CreatedAt = default,
    TimeSpan? Duration = null,
    bool Closable = true,
    bool ShowProgress = true
)
{
    public DateTimeOffset CreatedAtOrNow => CreatedAt == default ? DateTimeOffset.UtcNow : CreatedAt;
}