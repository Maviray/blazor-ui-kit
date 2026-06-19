using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Material.Constants;

namespace Maviray.Blazor.Components.Material.Extensions;

public static class EnumExtensions
{
    public static string GetBadgeCss(this ThemeColorScheme theme)
    {
        return theme switch
        {
            ThemeColorScheme.Primary => Tailwind.Theme.Badge.PRIMARY,
            ThemeColorScheme.Secondary => Tailwind.Theme.Badge.PRIMARY,
            ThemeColorScheme.Success => Tailwind.Theme.Badge.PRIMARY,
            ThemeColorScheme.Alert => Tailwind.Theme.Badge.PRIMARY,
            ThemeColorScheme.Warning => Tailwind.Theme.Badge.PRIMARY,
            ThemeColorScheme.Info => Tailwind.Theme.Badge.PRIMARY,
            ThemeColorScheme.Dark => Tailwind.Theme.Badge.PRIMARY,
            ThemeColorScheme.Light => Tailwind.Theme.Badge.PRIMARY,
            _ => Tailwind.Theme.Badge.DEFAULT
        };
    }
}