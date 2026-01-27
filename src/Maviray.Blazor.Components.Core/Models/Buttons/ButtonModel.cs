using Maviray.Blazor.Components.Core.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Maviray.Blazor.Components.Core.Models.Buttons
{
    public class ButtonModel
    {
        public string? Id { get; set; } = $"button_{Guid.NewGuid()}";

        public ButtonRole ButtonRole { get; set; }

        public ButtonType ButtonType { get; set; }

        public ElementSize ElementSize { get; set; }

        public TextTransform TextTransform { get; set; }

        public ButtonVariant ButtonVariant { get; set; }

        public bool Disabled { get; set; }

        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public string Width { get; set; } = "min-w-16";

        public string? StartIcon { get; set; }

        public string? EndIcon { get; set; }
    }
}
