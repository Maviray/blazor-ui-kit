using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Core.Models.Buttons;

public class IconButtonModel
{
     public string? Icon { get; set; }

     public ButtonRole ButtonRole { get; set; }

     public ThemeColorScheme ThemeColorScheme { get; set; }

     public ElementSize ElementSize { get; set; }

     public TextTransform TextTransform { get; set; }

     public ElementVariant ElementVariant { get; set; }

     public bool Disabled { get; set; }
}