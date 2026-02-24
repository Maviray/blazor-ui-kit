using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Extensions;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Components.DataDisplay;

public abstract class MaviDialogBase : MaviComponentBase
{
    protected bool Visible;

    [Parameter]
    public MaviDialogBaseParameters MaviDialogBaseParameters { get; set; } = new();

    [Parameter] public EventCallback<DialogButtonClickEventArgs> DialogButtonClick { get; set; }

    protected async Task HandleButtonClick(MouseClickEventArgs mouseEventArgs, DialogButtonClick buttonClicked)
    {
        if (EnableLifeCycleLogging)
        {
           Logger?.LogDebugLifeCycle(GetType(), Id, $"DialogButtonClick: {buttonClicked}");
        }

        if (DialogButtonClick.HasDelegate)
        {
            await DialogButtonClick.InvokeAsync(new(Id, buttonClicked, mouseEventArgs));
        }

        if (buttonClicked == Core.Enums.DialogButtonClick.Close)
        {
            Visible = false;
        }
    }

    public async Task Display(MaviDialogBaseParameters parameters)
    {
        MaviDialogBaseParameters = parameters;
        await Display();
    }

    public async Task Display(string title)
    {
        Title = title;
        await Display();
    }

    public Task Display()
    {
        if (Visible)
        {
            return Task.CompletedTask;
        }

        Visible = true;

        StateHasChanged();

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        return Task.CompletedTask;
    }

    public Task Hide()
    {
        if (!Visible)
        {
            return Task.CompletedTask;
        }

        Visible = false;

        StateHasChanged();

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        return Task.CompletedTask;
    }
}