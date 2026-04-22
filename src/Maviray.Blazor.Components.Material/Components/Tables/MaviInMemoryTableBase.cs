using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Constants;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Extensions;
using Maviray.Blazor.Components.Core.Models.Tables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Maviray.Blazor.Components.Material.Components.Tables;

public abstract class MaviInMemoryTableBase : MaviComponentBase, IAsyncDisposable
{
    const string HANDLE_OUTSIDE_CLICK_METHOD_TITLE = "HandleOutsideClick";

    protected TableDataCollection? Collection;
    protected IEnumerable<MaviTableContextMenuItem>? MainMenuItems;
    protected bool ContextMenuVisible;

    private System.Threading.Timer? _filterDebounceTimer;

    private DotNetObjectReference<MaviInMemoryTableBase>? _dotNetRef;

    [Parameter]
    public Func<Task<TableDataCollection>>? FetchData { get; set; }

    [Parameter]
    public Func<Task<IEnumerable<MaviTableContextMenuItem>>>? ConstructContextMenu { get; set; }

    [Parameter]
    public TableParameters? Parameters { get; set; }

    [Parameter]
    public EventCallback<MaviTableContextMenuItem> OnMainContextMenuClick { get; set; }

    [Parameter]
    public EventCallback<TableClickData> OnRowClick { get; set; }

    [Parameter]
    public EventCallback<MaviTableRowContextMenuItem> OnRowContextMenuClick { get; set; }

    [Parameter]
    public RenderFragment? NoDataContent { get; set; }

    [Inject]
    protected IJSRuntime? JsRuntime { get; set; }

    public TableRowContextMenuDisplayStyle TableRowContextMenuDisplayStyle => Parameters?.TableRowContextMenuDisplayStyle ?? TableRowContextMenuDisplayStyle.DropDown;
    public string ContextMenuId => $"context-menu-{Id}";

    protected IEnumerable<MaviTableColumn> VisibleColumns =>
        Collection?.Columns.Where(c => c.Visible).OrderBy(c => c.Sequence)
        ?? Enumerable.Empty<MaviTableColumn>();

    protected BackdropOpacity BackdropOpacity => Parameters?.BackdropOpacity ?? BackdropOpacity.Lighten;
    protected ZIndex ZIndex => Parameters?.ZIndex ?? ZIndex.Forty;

    public virtual int TotalEffectiveColumnsNumber
    {
        get
        {
            var collectionColumns = Collection?.Columns.Count(c => c.Visible) ?? 0;
            return collectionColumns + 1; // +1 for action column
        }
    }

    // public List<MaviTableRow> Rows => DataCollection?.Rows;

    protected override async Task OnInitializedAsync()
    {
        await RefreshTable();

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SubscribeElements();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Parameters ??= new();

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }
    }

    public virtual async Task Refresh()
    {
        await RefreshTable();
        StateHasChanged();
    }

    protected virtual async Task RefreshTable()
    {
        if (HasRendered)
        {
            await UnSubscribeCurrentRows();
        }

        await BuildCollection();
        await BuildContextMenu();

        if (HasRendered)
        {
            await SubscribeCurrentRows();
        }

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }
    }

    protected virtual async Task BuildCollection()
    {
        try
        {
            if (FetchData != null)
            {
                Collection = await FetchData.Invoke();
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, "Failed to fetch data for grid");
        }
    }

    protected virtual async Task BuildContextMenu()
    {
        if (ConstructContextMenu != null)
        {
            try
            {
                MainMenuItems = await ConstructContextMenu.Invoke();
            }
            catch (Exception ex)
            {
                Logger?.Error(ex, "Failed to construct main context menu.");
            }
        }
    }

    protected virtual string GetRowCss(MaviTableRow row) => string.Empty;

    protected virtual string GetCellTextCss(MaviTableColumn column) => string.Empty;

    protected virtual void SortByColumn(string columnKey)
    {
        Collection?.SortByColumn(columnKey);
    }

    protected virtual async Task PrevPage()
    {
        await UnSubscribeCurrentRows();

        if (Collection is not null && Collection.CurrentPage > 1)
        {
            Collection.CurrentPage--;
        }

        await SubscribeCurrentRows();
    }

    protected virtual async Task NextPage()
    {
        await UnSubscribeCurrentRows();

        if (Collection is not null && Collection.CurrentPage < Collection.TotalPages)
        {
            Collection.CurrentPage++;
        }

        await SubscribeCurrentRows();
    }

    protected virtual async Task UpdatePageSize(ChangeEventArgs e)
    {
        await UnSubscribeCurrentRows();

        if (Collection == null)
        {
            return;
        }

        if (!int.TryParse(e?.Value?.ToString(), out var newSize))
        {
            return;
        }

        Collection.PageSize = newSize;
        Collection.CurrentPage = 1;

        await SubscribeCurrentRows();
    }

    protected virtual void ToggleMainContextMenuDropDown(MouseClickEventArgs mouseEventArgs)
    {
        ContextMenuVisible = !ContextMenuVisible;
    }

    protected virtual void ToggleRowContextMenuDropDown(MaviTableRow row, MouseClickEventArgs mouseEventArgs)
    {
        row.ContextMenuVisible = !row.ContextMenuVisible;
    }

    protected virtual async Task ContextMenuClick(MaviTableContextMenuItem? item)
    {
        ContextMenuVisible = false;

        if (OnMainContextMenuClick.HasDelegate)
        {
            await OnMainContextMenuClick.InvokeAsync(item);
        }

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }
    }

    protected virtual async Task RowClick(MaviTableRow row, MaviTableColumn column)
    {
        if (OnRowClick.HasDelegate)
        {
            await OnRowClick.InvokeAsync(new(row, column));
        }

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }
    }

    protected virtual async Task RowContextMenuItemClick(MouseEventArgs args, MaviTableRow row, MaviTableRowContextMenuItem? action)
    {
        row.ContextMenuVisible = false;

        if (OnRowContextMenuClick.HasDelegate)
        {
            await OnRowContextMenuClick.InvokeAsync(action);
        }

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }
    }

    protected virtual void UpdateColumnFilter(string columnKey, ChangeEventArgs args)
    {
        _filterDebounceTimer?.Dispose();

        _filterDebounceTimer = new (_ =>
        {
            InvokeAsync(() =>
            {
                var filterText = args.Value?.ToString() ?? string.Empty;

                Collection?.SetColumnFilter(columnKey, filterText);

                StateHasChanged();
            });
        }, null, 300, Timeout.Infinite);

        if (EnableLifeCycleLogging)
        {
            Logger?.LogDebugLifeCycle(Id, GetType());
        }
    }
    
    #region handle outside click

    protected async Task SubscribeElements()
    {
        try
        {
            _dotNetRef = DotNetObjectReference.Create(this);

            if (JsRuntime != null)
            {
                await JsRuntime.InvokeVoidAsync(JsInteropConstants.REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, ContextMenuId, _dotNetRef, HANDLE_OUTSIDE_CLICK_METHOD_TITLE);

                await SubscribeCurrentRows();
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, ex.Message);
        }
    }

    protected async Task SubscribeCurrentRows()
    {
        try
        {
            if (JsRuntime != null)
            {
                if (Collection != null)
                {
                    foreach (var row in Collection.GetCurrentPageRows())
                    {
                        await JsRuntime.InvokeVoidAsync(JsInteropConstants.REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, row.ContextMenuId, _dotNetRef, HANDLE_OUTSIDE_CLICK_METHOD_TITLE);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, ex.Message);
        }
    }

    protected async Task UnSubscribeCurrentRows()
    {
        try
        {
            if (JsRuntime != null)
            {
                if (Collection != null)
                {
                    foreach (var row in Collection.GetCurrentPageRows())
                    {
                        await JsRuntime.InvokeVoidAsync(JsInteropConstants.UN_REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, row.ContextMenuId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, ex.Message);
        }
    }

    [JSInvokable]
    public void HandleOutsideClick(string elementId)
    {
        try
        {
            if (elementId == ContextMenuId)
            {
                ContextMenuVisible = false;
            }

            var row = Collection?.GetCurrentPageRows().FirstOrDefault(r => r.ContextMenuId == elementId);
            row?.ContextMenuVisible = false;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, ex.Message);
        }
    }

    private bool _disposed;

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_disposed)
            {
                return;
            }

            if (HasRendered && JsRuntime != null)
            {
                await JsRuntime.InvokeVoidAsync(JsInteropConstants.UN_REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, ContextMenuId);

                await UnSubscribeCurrentRows();
            }

            _dotNetRef?.Dispose();

            if (_filterDebounceTimer != null)
            {
                await _filterDebounceTimer.DisposeAsync();
            }

            _disposed = true;
            GC.SuppressFinalize(this);

            if (EnableLifeCycleLogging)
            {
                Logger?.LogDebugLifeCycle(Id, GetType());
            }
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, ex.Message);
        }
    }

    #endregion
}