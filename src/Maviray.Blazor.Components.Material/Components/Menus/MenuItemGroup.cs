namespace Maviray.Blazor.Components.Material.Components.Menus;

public class MenuItemGroup : MenuItemBase
{
    public bool Expanded { get; set; }
    public List<MenuItem> Items { get; set; } = [];

    public override void MarkActive(string? guid)
    {
        if (Guid == guid || Items.Any(x => x.Guid == guid))
        {
            Selected = true;
            Expanded = true;
        }
        else
        {
            Selected = false;
            Expanded = false;
        }

        foreach (var item in Items)
        {
            item.MarkActive(guid);
        }
    }
}