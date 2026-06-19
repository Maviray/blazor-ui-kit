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
            ThemeColorScheme.Secondary => Tailwind.Theme.Badge.SECONDARY,
            ThemeColorScheme.Success => Tailwind.Theme.Badge.SUCCESS,
            ThemeColorScheme.Alert => Tailwind.Theme.Badge.ALERT,
            ThemeColorScheme.Warning => Tailwind.Theme.Badge.WARNING,
            ThemeColorScheme.Info => Tailwind.Theme.Badge.INFO,
            ThemeColorScheme.Dark => Tailwind.Theme.Badge.DARK,
            ThemeColorScheme.Light => Tailwind.Theme.Badge.LIGHT,
            _ => Tailwind.Theme.Badge.DEFAULT
        };
    }
}