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
    protected TableDataCollection? Collection;
    protected IEnumerable<MaviTableContextMenuItem>? MainMenuItems;
    protected bool ContextMenuVisible;

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

    [Inject]
    protected IJSRuntime? JsRuntime { get; set; }

    public TableRowContextMenuDisplayStyle TableRowContextMenuDisplayStyle => Parameters?.TableRowContextMenuDisplayStyle ?? TableRowContextMenuDisplayStyle.DropDown;
    public string ContextMenuId => $"context-menu-{Id}";

    // public List<MaviTableRow> Rows => DataCollection?.Rows;

    protected override async Task OnInitializedAsync()
    {
        await RefreshTable();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SubscribeElements();
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Parameters ??= new();
    }

    public virtual async Task Refresh()
    {
        await RefreshTable();
        StateHasChanged();
    }

    protected virtual async Task RefreshTable()
    {
        await BuildCollection();
        await BuildContextMenu();
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
        if (Collection == null)
        {
            return;
        }

        if (Collection.SortColumnKey == columnKey)
        {
            Collection.SortOrder = Collection.SortOrder == SortOrder.Ascending
                ? SortOrder.Descending
                : SortOrder.Ascending;
        }
        else
        {
            Collection.SortColumnKey = columnKey;
            Collection.SortOrder = SortOrder.Ascending;
        }
    }

    protected virtual void PrevPage()
    {
        if (Collection is not null && Collection.CurrentPage > 1)
        {
            Collection.CurrentPage--;
        }
    }

    protected virtual void NextPage()
    {
        if (Collection is not null && Collection.CurrentPage < Collection.TotalPages)
        {
            Collection.CurrentPage++;
        }
    }

    protected virtual void UpdatePageSize(ChangeEventArgs e)
    {
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
        if (OnMainContextMenuClick.HasDelegate)
        {
            await OnMainContextMenuClick.InvokeAsync(item);
        }

        ContextMenuVisible = false;
    }

    protected virtual async Task RowClick(MaviTableRow row, MaviTableColumn column)
    {
        if (OnRowClick.HasDelegate)
        {
            await OnRowClick.InvokeAsync(new(row, column));
        }
    }

    protected virtual async Task RowContextMenuItemClick(MouseEventArgs args, MaviTableRowContextMenuItem? action)
    {
        if (OnRowContextMenuClick.HasDelegate)
        {
            await OnRowContextMenuClick.InvokeAsync(action);
        }
    }

    #region handle outside click

    private async Task SubscribeElements()
    {
        try
        {
            const string handleOutsideClickMethodTitle = "HandleOutsideClick";

            _dotNetRef = DotNetObjectReference.Create(this);

            if (JsRuntime != null)
            {
                await JsRuntime.InvokeVoidAsync(JsInteropConstants.REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, ContextMenuId, _dotNetRef, handleOutsideClickMethodTitle);

                if (Collection != null)
                {
                    foreach (var row in Collection.GetCurrentPageRows())
                    {
                        await JsRuntime.InvokeVoidAsync(JsInteropConstants.REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, row.ContextMenuId, _dotNetRef, handleOutsideClickMethodTitle);
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
        await DisposeAsync(true);

        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        try
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (JsRuntime != null)
                {
                    await JsRuntime.InvokeVoidAsync(JsInteropConstants.UN_REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, ContextMenuId);

                    if (Collection != null)
                    {
                        foreach (var row in Collection.GetCurrentPageRows())
                        {
                            await JsRuntime.InvokeVoidAsync(JsInteropConstants.UN_REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, row.ContextMenuId);
                        }
                    }
                }

                _dotNetRef?.Dispose();
            }

            _disposed = true;
        }
        catch (Exception ex)
        {
            Logger?.Error(ex, ex.Message);
        }
    }

    #endregion
}