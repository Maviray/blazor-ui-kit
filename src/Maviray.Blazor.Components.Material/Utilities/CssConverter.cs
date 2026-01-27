using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Material.Utilities
{
    internal static class CssConverter
    {
        public static string GetButtonClass(ButtonVariant buttonVariant, ButtonType buttonType)
        {
            return buttonVariant switch
            {
                ButtonVariant.Filled => GetFilledButtonClass(buttonType),
                ButtonVariant.Outlined => GetOutlinedButtonClass(buttonType),
                ButtonVariant.Text => GetTextButtonClass(buttonType),
                _ => GetFilledButtonClass(buttonType)
            };
        }

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

        public static string GetTextButtonClass(ButtonType buttonType)
        {
            return string.Empty;
        }

        public static string GetOutlinedButtonClass(ButtonType buttonType)
        {
            return buttonType switch
            {
                ButtonType.Default => "mavi-button-outlined mavi-button-outlined-default",
                ButtonType.Primary => "mavi-button-outlined mavi-button-outlined-primary",
                ButtonType.Secondary => "mavi-button-outlined mavi-button-outlined-secondary",
                ButtonType.Success => "mavi-button-outlined mavi-button-outlined-success",
                ButtonType.Danger => "mavi-button-outlined mavi-button-outlined-danger",
                ButtonType.Warning => "mavi-button-outlined mavi-button-outlined-warning",
                ButtonType.Info => "mavi-button-outlined mavi-button-outlined-info",
                ButtonType.Dark => "mavi-button-outlined mavi-button-outlined-dark",
                ButtonType.Light => "mavi-button-outlined mavi-button-outlined-light",
                _ => "mavi-button-outlined mavi-button-outlined-default"
            };
        }

        public static string GetTextTransformClass(TextTransform textTransform)
        {
            return textTransform switch
            {
                TextTransform.Normal => "normal-case",
                TextTransform.UpperCase => "uppercase",
                TextTransform.LowerCase => "lowercase",
                TextTransform.Capitalize => "capitalize",
                _ => "uppercase" // Default to uppercase for Material Design
            };
        }

        public static string GetButtonSizeClass(ElementSize size)
        {
            return size switch
            {
                ElementSize.Small => "mavi-button-filled-small",
                ElementSize.Regular => "mavi-button-filled-regular",
                ElementSize.Large => "mavi-button-filled-large",
                _ => "mavi-button-filled-regular"
            };
        }
    }
}
