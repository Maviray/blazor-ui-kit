using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Models.Toastr;

namespace Maviray.Blazor.Components.Core.Services;

public interface IMaviToastrService
{
    /// <summary>
    /// Currently active toasts, newest first.
    /// </summary>
    IReadOnlyList<ToastItem> ActiveToasts { get; }

    /// <summary>
    /// Raised whenever the active toast list changes (add/remove).
    /// </summary>
    event Action? OnChanged;

    /// <summary>
    /// Shows a fully-configured toast.
    /// </summary>
    void Show(ToastItem item);

    /// <summary>
    /// Shows a toast with the given message and optional overrides.
    /// </summary>
    void Show(
        string message,
        string? title = null,
        ThemeColorScheme colorScheme = ThemeColorScheme.Default,
        ElementVariant variant = ElementVariant.Filled,
        ElementSize size = ElementSize.Regular,
        ToastrPosition position = ToastrPosition.CenterTop,
        int duration = 3000,
        string? icon = null);

    /// <summary>
    /// Removes the toast with the given id.
    /// </summary>
    void Dismiss(Guid id);

    /// <summary>
    /// Removes all active toasts.
    /// </summary>
    void DismissAll();
}
