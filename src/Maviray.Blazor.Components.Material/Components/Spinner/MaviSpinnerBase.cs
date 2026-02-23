using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Components.Spinner;

public class MaviSpinnerBase : MaviComponentBase
{
    [Parameter] public ElementSize ElementSize { get; set; } = ElementSize.Regular;
    [Parameter] public ThemeColorScheme ThemeColorScheme { get; set; } = ThemeColorScheme.Default;

    /// <summary>
    /// Pass double scale value to override the default size of the spinner. Default size is determined by the ElementSize parameter. Scale value should be between 0 and 1, where 0.5 means half the default size and 1 means the default size. If Scale is set to 0, the default size will be used based on the ElementSize parameter.
    /// </summary>
    [Parameter] public double Scale { get; set; } = 0;

    protected double ScaleElement => Scale != 0 && Scale is >= -1 and <= 1 ? Scale : ElementSize switch
    {
        ElementSize.Small => 0.5,
        ElementSize.Large => 1,
        _ => 0.75
    };
}