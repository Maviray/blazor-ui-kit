using Maviray.Blazor.Components.Core.Enums;
using Maviray.Blazor.Components.Core.Interfaces;
using Maviray.Blazor.Components.Core.Services;
using Maviray.Blazor.Components.Material.Components.Menus;

namespace Maviray.Blazor.Components.Samples.Material.Client.Services;

public class SampleCircuitStateService : CircuitStateService
{
    public override Task<IEnumerable<IMenuItem>> GetMenuItems()
    {
        var list = new List<IMenuItem>
        {
            new MenuItemGroup()
            {
                Title = "Base",
                Icon = "lni lni-hand-taking-user",
                BadgeColor = ThemeColorScheme.Alert,
                BadgeText = "7",
                Items =
                [
                    new()
                    {
                        Title = "Theme Colors",
                        Icon = "lni lni-colour-palette-3",
                        NavigateTo = "PageCustomColors"
                    },
                    new()
                    {
                        Title = "Screen Sizes",
                        Icon = "lni lni-sports",
                        NavigateTo = "PageScreenSizes"

                    },
                    new()
                    {
                        Title = "Icons Mini",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageIconsMini"

                    },
                    new()
                    {
                        Title = "Icons Online",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageIconsOnline"

                    },
                    new()
                    {
                        Title = "Icons Colors",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageIconColors"

                    },
                    new()
                    {
                        Title = "Line Icons",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageLineIcons"

                    },
                    new()
                    {
                        Title = "Spinners",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviSpinners"

                    }
                ]
            },
            new MenuItemGroup()
            {
                Title = "Inputs",
                Icon = "lni lni-hand-taking-user",
                BadgeColor = ThemeColorScheme.Alert,
                BadgeText = "7",
                Items =
                [
                    new()
                    {
                        Title = "Buttons",
                        Icon = "lni lni-colour-palette-3",
                        NavigateTo = "PageMaviButton"
                    },
                    new()
                    {
                        Title = "Button Groups",
                        Icon = "lni lni-sports",
                        NavigateTo = "PageMaviButtonGroup"

                    },
                    new()
                    {
                        Title = "Input Integer",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputInteger"

                    },
                    new()
                    {
                        Title = "Input Integer Nullable",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputIntegerNullable"

                    },
                    new()
                    {
                        Title = "Input String",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputString"

                    },
                    new()
                    {
                        Title = "Input String Nullable",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputStringNullable"

                    },
                    new()
                    {
                        Title = "Input Double",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputDouble"

                    },
                    new()
                    {
                        Title = "Input Double Nullable",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputDoubleNullable"

                    },
                    new()
                    {
                        Title = "Input Decimal",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputDecimal"

                    },
                    new()
                    {
                        Title = "Input Long",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputLong"

                    },
                    new()
                    {
                        Title = "Input Float",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputFloat"

                    },
                    new()
                    {
                        Title = "Input Short",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputShort"

                    },
                    new()
                    {
                        Title = "Input Date",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputDate"

                    },
                    new()
                    {
                        Title = "Input Date Nullable",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviInputDateNullable"

                    },
                    new()
                    {
                        Title = "Dropdown",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviDropdown"

                    },
                    new()
                    {
                        Title = "MultiSelect",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviMultiSelect"

                    },
                    new()
                    {
                        Title = "Checkbox",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviCheckbox"

                    },
                    new()
                    {
                        Title = "Toggle",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviToggle"

                    },
                    new()
                    {
                        Title = "Switch",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviSwitch"

                    },
                    new()
                    {
                        Title = "Autocomplete",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviAutocomplete"

                    },
                    new()
                    {
                        Title = "Slider",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviSlider"

                    },
                    new()
                    {
                        Title = "Radio Button",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageRadioButton"

                    },
                    new()
                    {
                        Title = "Radio Group",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviRadioGroup"

                    },
                    new()
                    {
                        Title = "Links",
                        Icon = "lni lni-thumbs-up-3",
                        NavigateTo = "PageMaviLink"

                    }
                ]
            }
        };

        return Task.FromResult(list.AsEnumerable());
    }
}