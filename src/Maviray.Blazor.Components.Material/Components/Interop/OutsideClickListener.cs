using Microsoft.JSInterop;

namespace Maviray.Blazor.Components.Material.Components.Interop;

/// <summary>
///     Keeps a popup's document click listener attached only while the popup is open, so a closed
///     popup costs no JS interop when the user clicks elsewhere on the page.
///     Call <see cref="SyncAsync" /> after every render.
/// </summary>
internal sealed class OutsideClickListener
{
    private bool _syncing;

    public bool IsAttached { get; private set; }

    /// <summary>
    ///     Attaches or detaches the listener until it matches <paramref name="shouldListen" />. The open
    ///     state is re-read after each await, so a change made meanwhile is applied by the running call.
    /// </summary>
    public async Task SyncAsync(Func<bool> shouldListen, Func<ValueTask> attach, Func<ValueTask> detach)
    {
        if (_syncing)
        {
            return;
        }

        _syncing = true;

        try
        {
            while (IsAttached != shouldListen())
            {
                if (IsAttached)
                {
                    await detach();
                    IsAttached = false;
                }
                else
                {
                    await attach();
                    IsAttached = true;
                }
            }
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone, so there is no browser listener left to manage.
        }
        finally
        {
            _syncing = false;
        }
    }
}
