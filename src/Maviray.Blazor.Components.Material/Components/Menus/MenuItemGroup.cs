namespace Maviray.Blazor.Components.Material.Components.Menus;

public class MenuItemGroup : MenuItemBase
{
    public bool Expanded { get; set; }
    public List<MenuItem> Items { get; set; } = [];

    public override void MarkActive(string? guid)
    {
        var clickPartOfGroup = false;

        // click on group title
        if (Guid == guid)
        {
            Selected = true;
            Expanded = !Expanded;

            clickPartOfGroup = true;
        }
        else
        {
            // click on one of group sub-items
            if (Items.Any(x => x.Guid == guid))
            {
                Selected = true;
                Expanded = true;

                clickPartOfGroup = true;
            }
        }

        if (!clickPartOfGroup)
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