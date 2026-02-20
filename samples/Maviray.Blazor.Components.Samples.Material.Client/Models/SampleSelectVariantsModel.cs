using Maviray.Blazor.Components.Core.Enums;
namespace Maviray.Blazor.Components.Samples.Material.Client.Models;

public class SampleSelectVariantsModel
{
    public string? Title { get; set; }
    public string? Text { get; set; }

    public ThemeColorScheme ThemeColorScheme { get; set; }

    public ElementVariant ElementVariant { get; set; }

    public ElementSize ElementSize { get; set; } 
}