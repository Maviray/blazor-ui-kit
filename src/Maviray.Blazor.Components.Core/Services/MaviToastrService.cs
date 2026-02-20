using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Models.Toastr;
using Maviray.Blazor.Components.Core.Options;

namespace Maviray.Blazor.Components.Core.Services;

public class MaviToastrService : IMaviToastrService
{
    private readonly MaviToastrOptions _options;
    private readonly List<ToastItem> _toasts = [];
    private readonly object _lock = new();

    public event Action? OnChanged;

    public MaviToastrService(MaviToastrOptions options)
    {
        _options = options;
    }

    public IReadOnlyList<ToastItem> ActiveToasts
    {
        get
        {
            lock (_lock)
            {
                return _toasts.ToList();
            }
        }
    }

    public void Show(
        string message,
        string? title = null,
        ThemeColorScheme colorScheme = ThemeColorScheme.Default,
        ElementVariant variant = ElementVariant.Filled,
        ElementSize size = ElementSize.Regular,
        ToastrPosition position = ToastrPosition.CenterTop,
        int duration = 3000,
        string? icon = null)
    {
        Show(new ToastItem
        {
            Message = message,
            Title = title,
            ColorScheme = colorScheme,
            Variant = variant,
            Size = size,
            Position = position,
            Duration = duration,
            Icon = icon
        });
    }

    public void Show(ToastItem item)
    {
        lock (_lock)
        {
            _toasts.Insert(0, item);

            if (_options.MaxVisibleCount.HasValue)
            {
                while (_toasts.Count > _options.MaxVisibleCount.Value)
                {
                    _toasts.RemoveAt(_toasts.Count - 1);
                }
            }
        }

        OnChanged?.Invoke();

        if (!item.IsSticky)
        {
            _ = DismissAfterAsync(item.Id, item.Duration);
        }
    }

    public void Dismiss(Guid id)
    {
        bool changed;
        lock (_lock)
        {
            changed = _toasts.RemoveAll(t => t.Id == id) > 0;
        }

        if (changed)
        {
            OnChanged?.Invoke();
        }
    }

    public void DismissAll()
    {
        lock (_lock)
        {
            _toasts.Clear();
        }

        OnChanged?.Invoke();
    }

    private async Task DismissAfterAsync(Guid id, int duration)
    {
        await Task.Delay(duration);
        Dismiss(id);
    }
}
