using Maviray.Blazor.Components.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maviray.Blazor.Components.Material.Utilities
{
    internal static class CssConverter
    {
        public static string GetFilledButtonClass(ButtonType buttonType)
        {
            return buttonType switch
            {
                ButtonType.Default => "mavi-button-filled mavi-button-filled-default",
                ButtonType.Primary => "mavi-button-filled mavi-button-filled-primary",
                ButtonType.Secondary => "mavi-button-filled mavi-button-filled-secondary",
                ButtonType.Success => "mavi-button-filled mavi-button-filled-success",
                ButtonType.Danger => "mavi-button-filled mavi-button-filled-danger",
                ButtonType.Warning => "mavi-button-filled mavi-button-filled-warning",
                ButtonType.Info => "mavi-button-filled mavi-button-filled-info",
                ButtonType.Dark => "mavi-button-filled mavi-button-filled-dark",
                ButtonType.Light => "mavi-button-filled mavi-button-filled-light",
                _ => "mavi-button-filled mavi-button-filled-default"
            };
        }
    }
}
