using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Maviray.Blazor.Components.Material.Components.Inputs
{
    public class MaviButtonBase : MaviComponentBase
    {
        [Parameter] public override string? Id { get; set; } = $"button_{Guid.NewGuid()}";

        [Parameter] public ButtonRole ButtonRole { get; set; }

        [Parameter] public ButtonType ButtonType { get; set; }

        [Parameter] public ElementSize ElementSize { get; set; }

        [Parameter] public TextTransform TextTransform { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter] public string? IconCss { get; set; }

        [Parameter]
        public string? StartIcon { get; set; }

        [Parameter]
        public string? EndIcon { get; set; }
    }
}
