# Maviray Blazor UI Kit

Blazor UI components with a Material Design look, styled with Tailwind CSS 4.

The `Mavi*` components cover forms, buttons, dialogs, toasts, data tables, menus, tabs and a rich-text editor. A shared
theme of CSS variables, plus C# constants for its Tailwind classes, gives your own markup the same palette, type scale and
interaction states as the components. The packages ship prebuilt CSS, so using the components needs no Node.js and no
Tailwind build.

- **.NET 10**, for Blazor apps with interactive Server, WebAssembly or Auto rendering
- **Forms that validate**: inputs extend Blazor's `InputBase`, so `@bind-Value`, `EditForm` and data-annotation validation
  work as usual, and labels and required markers come from your model's attributes
- **Themeable at runtime**: 12 color roles × 10 shades, Material type scale and elevation
- **Ready for trimmed WebAssembly builds**: the libraries are trimmable and trim-analyzer clean

## Packages

| Package | Contents |
|---------|----------|
| `Maviray.Blazor.Components.Material` | The `Mavi*` components, the Material theme and the `Tailwind.Theme` constants. **Install this one.** |
| `Maviray.Blazor.Components.Core` | Base classes, enums, models and services. Installed with Material. |
| `Maviray.Blazor.Components.Tailwind` | A Tailwind-native theme stylesheet and its constants. Its components are planned; there are none yet. |

## Components

### Inputs and forms

| Component | What it is |
|-----------|------------|
| `MaviInputString`, `MaviInputStringNullable` | Text field: text, password (with show/hide), email, phone, URL or search |
| `MaviTextArea` | Multi-line text, with an optional character counter |
| `MaviInputInteger`, `…IntegerNullable`, `…Long`, `…Short`, `…Float`, `…Double`, `…DoubleNullable`, `…Decimal` | Number fields with min/max, step, format masks and culture-aware formatting |
| `MaviInputDate`, `MaviInputDateNullable` | Date field with a calendar picker |
| `MaviDatePicker` | The calendar on its own |
| `MaviCheckbox`, `MaviSwitch`, `MaviToggle`, `MaviRadioButton` | On/off inputs |
| `MaviRadioGroup`, `MaviRadioGroupItem` | One choice from a set |
| `MaviDropdown` | Single select; fills itself from an enum |
| `MaviMultiSelect` | Multiple select with chips and select-all / clear-all |
| `MaviAutoComplete` | Type-ahead search over an async source, with debounce and keyboard navigation |
| `MaviSlider` | Range slider |
| `MaviValidationMessage` | An error message line you show from code |

### Buttons and navigation

| Component | What it is |
|-----------|------------|
| `MaviButton`, `MaviButtonIcon`, `MaviButtonGroup` | Filled, outlined and text buttons; icon buttons; segmented button groups |
| `MaviLink` | A styled `NavLink` |
| `MaviTabNavBar`, `MaviTab`, `MaviTabContent` | Tabs |
| `MaviMenuVertical` | Collapsible sidebar menu with nested groups and badges |
| `MaviDropDownMenu` | A button that opens an action menu |
| `TopNavBar` | Top app bar with left, center and right slots |

### Data display

| Component | What it is |
|-----------|------------|
| `MaviInMemoryTable`, `MaviCssGridTable` | Data grids with sorting, column filters, paging, and toolbar and row menus. Build the data from `[TableColumn]` attributes on your row type; a helper exports it as CSV. |
| `MaviBadge`, `MaviChip` | Status badges; chips and tags |
| `MaviIconMini`, `MaviIconSolid`, `MaviIconOnline` | 859 built-in SVG icons in three sets |

### Feedback and overlays

| Component | What it is |
|-----------|------------|
| `MaviAlert` | Inline alert banner |
| `MaviToastr` | Toast notifications with auto-dismiss and a progress bar |
| `MaviDialog` | Modal dialog with confirm/cancel buttons and a busy state |
| `MaviBackdrop` | Loading overlay |
| `MaviSkeleton` | Loading placeholders |
| `MaviSpinner`, `…Equalizer`, `…Grid`, `…Roller`, `…Star`, `…StarRotate` | Six spinner styles |

### Surfaces, editor and errors

| Component | What it is |
|-----------|------------|
| `MaviCard`, `MaviPanel` | Card and titled panel |
| `MaviAccordion`, `MaviAccordionItem` | Expandable sections |
| `WysiwygEditor` | Rich-text HTML editor: headings, lists, fonts, colors, links, inline images, tables, code view and fullscreen. HTML is sanitized going in and out. |
| `MaviErrorHandler` | Error boundary for the app root |

Every component's parameters, methods and events are documented in the
[component reference](https://github.com/Maviray/blazor-ui-kit/tree/main/plugins/maviray-material/skills/maviray-blazor-components).

## Installation

The packages are published to GitHub Packages, which needs a GitHub token even to download them.

1. Create a GitHub personal access token (classic) with the `read:packages` scope.
2. Store it once per machine. Run this **outside any repository**: inside one, the command writes the token into that
   repository's `nuget.config`.

   ```bash
   dotnet nuget add source "https://nuget.pkg.github.com/Maviray/index.json" --name maviray --username YOUR_GITHUB_USERNAME --password YOUR_TOKEN
   ```

   On macOS and Linux, add `--store-password-in-clear-text`; Windows encrypts the password.
3. In your app's repository, add a `nuget.config` that takes Maviray packages from that feed and everything else from
   nuget.org. Restores then work the same on every machine, and central package management doesn't warn about several
   feeds (NU1507). The `maviray` key must match the name used in step 2, which is how NuGet finds the token.

   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <packageSources>
       <clear />
       <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
       <add key="maviray" value="https://nuget.pkg.github.com/Maviray/index.json" />
     </packageSources>
     <packageSourceMapping>
       <packageSource key="nuget.org">
         <package pattern="*" />
       </packageSource>
       <packageSource key="maviray">
         <package pattern="Maviray.*" />
       </packageSource>
     </packageSourceMapping>
   </configuration>
   ```

4. Add the package:

   ```bash
   dotnet add package Maviray.Blazor.Components.Material
   ```

In GitHub Actions, add the source with `${{ secrets.GITHUB_TOKEN }}` as the password and give the job
`permissions: packages: read`.

## Getting started

**Register the services** in `Program.cs`. In a Blazor Web App that uses WebAssembly, do this in both the server and
the client project:

```csharp
using Maviray.Blazor.Components.Core.Extensions;

builder.Services.AddMaviComponents();
```

Every component needs this registration. Using `MaviMenuVertical` also needs
`builder.Services.AddScoped<ICircuitStateService, CircuitStateService>();` (from `Maviray.Blazor.Components.Core.Services`).

**Add the assets** to the `<head>` of `App.razor` (or `index.html`):

```html
<link rel="stylesheet" href="_content/Maviray.Blazor.Components.Material/css/maviray.material.min.css" />
<link rel="stylesheet" href="_content/Maviray.Blazor.Components.Material/Maviray.Blazor.Components.Material.bundle.scp.css" />
<script src="_content/Maviray.Blazor.Components.Material/js/maviJsInterop.js"></script>
```

The theme's fonts aren't bundled. Load Lato, Montserrat, Oswald and Roboto Serif (for example from Google Fonts), and
the [LineIcons](https://lineicons.com/) stylesheet, which the components' default icons use.

**Use the components** with an interactive render mode:

```razor
@using Maviray.Blazor.Components.Material.Components.Inputs
@using Maviray.Blazor.Components.Core.Enums

<EditForm Model="_model" OnValidSubmit="SaveAsync">
    <DataAnnotationsValidator />
    <MaviInputString @bind-Value="_model.Name" Width="w-96" />
    <MaviInputDate @bind-Value="_model.StartDate" />
    <MaviDropdown TItem="Plan" TValue="Plan" @bind-Value="_model.Plan" />
    <MaviButton ButtonRole="ButtonRole.Submit" Title="Save" ThemeColorScheme="ThemeColorScheme.Primary" />
</EditForm>
```

Text, number and date inputs update the bound value when the user leaves the field or presses Enter. Set
`Immediate="true"` to update on every keystroke.

## Theming

Components take strongly typed parameters: `ThemeColorScheme`, `ElementVariant` (filled, outlined, text) and
`ElementSize`. Pass extra Tailwind classes through `Class`.

For your own markup, use the theme's CSS variables (`bg-(--theme-primary-eight)`) or the matching C# constants
(`Tailwind.Theme.Colors.Background.THEME_PRIMARY_EIGHT_BG`). Every class the constants produce ships in the library's
stylesheet. Class strings you write yourself need your app's own Tailwind 4 build. The
[styling guide](https://github.com/Maviray/blazor-ui-kit/blob/main/plugins/maviray-material/skills/material-css-styling/SKILL.md)
covers the tokens, the type scale and that setup.

## Claude Code skills

The `maviray-material` plugin teaches Claude Code the Mavi* components and how to style them with the library's theme
tokens. It comes from the Maviray marketplace ([Maviray/claude-plugins](https://github.com/Maviray/claude-plugins)). To
give your whole team the skills, commit this to your project's `.claude/settings.json`:

```json
{
  "extraKnownMarketplaces": {
    "maviray": { "source": { "source": "github", "repo": "Maviray/claude-plugins" } }
  },
  "enabledPlugins": {
    "maviray-material@maviray": true
  }
}
```

Each developer then trusts the folder when Claude Code asks, and installs the plugin once in their clone:

```
claude plugin install maviray-material@maviray --scope project
```

The skills follow this repo's `main` branch, which matches the latest released package. `claude plugin update
maviray-material@maviray` picks up a newer version. The skill sources are in
[plugins/maviray-material](https://github.com/Maviray/blazor-ui-kit/tree/main/plugins/maviray-material).

## Building from source

You need the .NET 10 SDK (see `global.json`) and Node.js with npm, which builds the Tailwind CSS. To build without
Node.js using the committed CSS, add `-p:SkipTailwindBuild=true`.

```bash
dotnet build Maviray.Blazor.Components.slnx
dotnet test --project tests/Maviray.Blazor.Components.Material.Tests/Maviray.Blazor.Components.Material.Tests.csproj
dotnet run --project samples/Maviray.Blazor.Components.Samples.Material
```

The last command starts the showcase app, with demo pages for each component area.

## Status

The library is under active development and still below 1.0, so a minor version can contain breaking changes. Each
release's changes, with upgrade notes, are in the
[changelog](https://github.com/Maviray/blazor-ui-kit/blob/main/CHANGELOG.md).

Planned: rating, avatar, divider, tooltip, progress bar, breadcrumbs, pagination, stepper and layout components, and the
Tailwind-native component set.

## Contributing

Issues and pull requests are welcome.

## License

[MIT](https://github.com/Maviray/blazor-ui-kit/blob/main/LICENSE.txt). You can use, modify and distribute the library
for free, including commercially. Keep the copyright notice (Copyright (c) 2025-2026 Maviray) and the license text in
copies of the library.
