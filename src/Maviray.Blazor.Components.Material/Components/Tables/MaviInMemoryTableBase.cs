using Maviray.Blazor.Components.Core.Components;
using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.EventArgs;
using Maviray.Blazor.Components.Core.Extensions;
using Maviray.Blazor.Components.Core.Models.Tables;
using Maviray.Blazor.Components.Material.Constants;
using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Material.Components.Tables;

public abstract class MaviInMemoryTableBase : MaviComponentBase
{
    protected TableDataCollection? Collection;
    protected IEnumerable<MaviTableContextMenuItem>? MainMenuItems;
    protected bool _contextMenuVisible;

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

   // public List<MaviTableRow> Rows => DataCollection?.Rows;

    protected override async Task OnInitializedAsync()
    {
        await RefreshTable();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
           // process JsRuntime initialization
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

    protected virtual void ToggleContextMenuDropDown(MouseClickEventArgs mouseEventArgs)
    {
        _contextMenuVisible = !_contextMenuVisible;
    }

    protected virtual async Task ContextMenuClick(MaviTableContextMenuItem? item)
    {
        if (OnMainContextMenuClick.HasDelegate)
        {
            await OnMainContextMenuClick.InvokeAsync(item);
        }

        _contextMenuVisible = false;
    }

    protected virtual async Task RowClick(MaviTableRow row, MaviTableColumn column)
    {
        if (OnRowClick.HasDelegate)
        {
            await OnRowClick.InvokeAsync(new(row, column));
        }
    }

    protected virtual async Task RowContextMenuItemClick(MaviTableRowContextMenuItem action)
    {
        if (OnRowContextMenuClick.HasDelegate)
        {
            await OnRowContextMenuClick.InvokeAsync(action);
        }
    }
}