using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace Maviray.Blazor.Components.Material.Components.Inputs
{
    public class MaviButtonBase : MaviComponentBase
    {
        [Parameter] public override string? Id { get; set; } = $"button_{Guid.NewGuid()}";

        [Parameter] public ButtonRole ButtonRole { get; set; }

        [Parameter] public ButtonType ButtonType { get; set; }

        [Parameter] public ElementSize ElementSize { get; set; }

        [Parameter] public TextTransform TextTransform { get; set; }

        [Parameter] public ButtonVariant ButtonVariant { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public EventCallback<MouseClickEventArgs> OnClick { get; set; }

        protected string Role => ButtonRole.ToString().ToLower();

        protected bool IsLoading { get; set; } = false;

        protected async Task HandleButtonClick(MouseEventArgs args)
        {
            if (OnClick.HasDelegate)
            {
                if (IsLoading)
                {
                    Logger?.LogInformation("delegate executing. will quit.");
                    return;
                }

                IsLoading = true;
                StateHasChanged();

                try
                {
                    var customArgs = new MouseClickEventArgs(Id)
                    {
                        AltKey = args.AltKey,
                        Button = args.Button,
                        Buttons = args.Buttons,
                        ClientX = args.ClientX,
                        ClientY = args.ClientY,
                        CtrlKey = args.CtrlKey,
                        Detail = args.Detail,
                        MetaKey = args.MetaKey,
                        OffsetX = args.OffsetX,
                        OffsetY = args.OffsetY,
                        PageX = args.PageX,
                        PageY = args.PageY,
                        ScreenX = args.ScreenX,
                        ScreenY = args.ScreenY,
                        ShiftKey = args.ShiftKey,
                        Type = args.Type
                    };

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
