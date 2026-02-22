using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Components.Spinner;

public class MaviSpinnerBase : MaviComponentBase
{
    [Parameter] public ElementSize ElementSize { get; set; } = ElementSize.Regular;
    [Parameter] public ThemeColorScheme ThemeColorScheme { get; set; } = ThemeColorScheme.Default;
}