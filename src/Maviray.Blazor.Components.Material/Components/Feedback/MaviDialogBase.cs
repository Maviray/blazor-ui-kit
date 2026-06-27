using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Extensions;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Components.Feedback;

public abstract class MaviDialogBase : MaviComponentBase
{
    /// <summary>
    ///     Optional per-invocation callback supplied by the caller of
    ///     <see cref="Display(Func{Core.Enums.DialogButtonClick, Task})" />.
    ///     Only the delegate passed to the most recent <c>Display</c> call is retained, and its lifetime ends as soon as the
    ///     user reacts by clicking a button or the dialog is otherwise closed.
    /// </summary>
    private Func<DialogButtonClick, Task>? _onButtonClick;

    protected bool Visible;

    [Parameter]
    public MaviDialogBaseParameters MaviDialogBaseParameters { get; set; } = new();

    [Parameter]
    public EventCallback<DialogButtonClickEventArgs> DialogButtonClick { get; set; }

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

        // Capture and clear the callback before invoking so its lifetime ends with this user reaction.
        // Clearing first lets the callback itself re-open the dialog with a fresh delegate without it being wiped.
        var onButtonClick = _onButtonClick;
        _onButtonClick = null;

        if (onButtonClick is not null)
        {
            await onButtonClick(buttonClicked);
        }

        if (buttonClicked == Core.Enums.DialogButtonClick.Close || MaviDialogBaseParameters.CloseOnUserAction)
        {
            Visible = false;
        }
    }

    public async Task Display(MaviDialogBaseParameters parameters)
    {
        MaviDialogBaseParameters.Update(parameters);
        await Display();
    }

    public async Task Display(string title)
    {
        Title = title;
        await Display();
    }

    /// <summary>
    ///     Displays the dialog and registers a callback invoked when the user reacts to it.
    ///     Replaces any callback registered by a previous <c>Display</c> call (latest wins).
    /// </summary>
    public async Task Display(MaviDialogBaseParameters parameters, Func<DialogButtonClick, Task> onButtonClick)
    {
        _onButtonClick = onButtonClick;
        MaviDialogBaseParameters.Update(parameters);
        await Display();
    }

    /// <summary>
    ///     Displays the dialog with the given title and registers a callback invoked when the user reacts to it.
    ///     Replaces any callback registered by a previous <c>Display</c> call (latest wins).
    /// </summary>
    public async Task Display(string title, Func<DialogButtonClick, Task> onButtonClick)
    {
        _onButtonClick = onButtonClick;
        Title = title;
        await Display();
    }

    /// <summary>
    ///     Displays the dialog and registers a callback invoked when the user reacts to it.
    ///     Replaces any callback registered by a previous <c>Display</c> call (latest wins).
    /// </summary>
    public async Task Display(Func<DialogButtonClick, Task> onButtonClick)
    {
        _onButtonClick = onButtonClick;
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

        // The dialog is closing without a button reaction; end the pending callback's lifetime.
        _onButtonClick = null;

        Visible = false;

        StateHasChanged();

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }

        return Task.CompletedTask;
    }
}