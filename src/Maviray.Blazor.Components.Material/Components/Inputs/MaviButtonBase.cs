using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Maviray.Blazor.Components.Material.Components.Inputs
{
    public class MaviButtonBase : MaviAttributesComponent
    {
        [Parameter] public override string? Id { get; set; } = $"button_{Guid.NewGuid()}";

        [Parameter] public ButtonRole ButtonRole { get; set; }

        [Parameter] public ThemeColorScheme ThemeColorScheme { get; set; }

        [Parameter] public ElementSize ElementSize { get; set; }

        [Parameter] public TextTransform TextTransform { get; set; }

        [Parameter] public ElementVariant ElementVariant { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public EventCallback<MouseClickEventArgs> OnClick { get; set; }

        protected string Role => ButtonRole.ToString().ToLower();

        protected bool IsLoading { get; set; } = false;

        protected async Task HandleButtonClick(MouseEventArgs args)
        {
            if (EnableLifeCycleLogging)
            {
                Logger?.LogDebugLifeCycle( Id, GetType());
            }

            if (OnClick.HasDelegate)
            {
                if (IsLoading)
                {
                    return;
                }

                IsLoading = true;
                StateHasChanged();

                try
                {
                    var customArgs = new MouseClickEventArgs(Id, args);

                    await OnClick.InvokeAsync(customArgs);
                }
                finally
                {
                    IsLoading = false;
                    StateHasChanged();
                }
            }
        }
    }
}
