---
name: maviray-blazor-components
description: >-
  Use when building or changing UI in a .NET/Blazor app that consumes the
  Maviray.Blazor.Components.Material library (the Mavi* components) — whenever
  you need a form field, button, dialog, data table, tabs, menu, accordion,
  card, panel, alert, toast, spinner, badge, chip, icon, dropdown, multi-select,
  autocomplete, date picker, checkbox/switch/toggle/radio, slider, or any
  interactive UI element. Consult this BEFORE writing custom HTML or hand-rolled
  Blazor markup for a UI element: this library is the standard solution and
  almost always already ships the component, fully themed and accessible. Covers
  every component's parameters, exposed methods, data models, events, and usage
  patterns. Pair with `material-css-styling` for theming/Tailwind decisions.
---

# Maviray Blazor Material Components

## Overview

`Maviray.Blazor.Components.Material` is a Blazor component library (Mavi*
components) providing themed, accessible, Material-Design UI building blocks. It
targets .NET 8/9/10 and ships as a NuGet package with its own CSS + JS assets.

**Core principle for consuming projects:** reach for a `Mavi*` component first.
Almost every common UI need — inputs, buttons, dialogs, tables, menus, tabs,
cards, alerts, toasts, spinners — already exists here, wired for theming,
validation, and accessibility. Do **not** hand-roll `<input>`, `<button>`,
`<table>`, `<dialog>`, dropdown, or modal markup when a component covers it.
Configure the component through its strongly-typed parameters instead.

This skill documents the components' **API surface** (parameters, methods,
events, data models). For color/typography/spacing/Tailwind decisions, use the
companion **`material-css-styling`** skill.

## When to use

- Adding or editing any screen, form, list, dialog, or interactive element
- Choosing which `Mavi*` component fits a need ("is there a component for X?")
- Configuring a component's parameters, wiring its events, or calling its methods
- Feeding data into the table, menu, autocomplete, dropdown, or toast components
- Reviewing whether existing custom markup should be replaced by a component

## "I need X" → component quick lookup

| Need | Component(s) | Reference |
|------|--------------|-----------|
| Text field | `MaviInputString` / `MaviInputStringNullable` | inputs-forms |
| Number field | `MaviInputInteger`, `…Long`, `…Short`, `…Float`, `…Double`, `…Decimal` (+ `Nullable` variants) | inputs-forms |
| Multi-line text | `MaviTextArea` | inputs-forms |
| Checkbox / on-off toggle | `MaviCheckbox`, `MaviSwitch`, `MaviToggle` | inputs-forms |
| Single choice from a set | `MaviRadioGroup` + `MaviRadioButton`, or `MaviDropdown` | inputs-forms |
| Multiple choice | `MaviMultiSelect` | inputs-forms |
| Type-ahead / async search | `MaviAutoComplete` | inputs-forms |
| Date / time picker | `MaviInputDate` / `MaviInputDateNullable` (calendar: `MaviDatePicker`) | inputs-forms |
| Range slider | `MaviSlider` | inputs-forms |
| Manual field error message | `MaviValidationMessage` | inputs-forms |
| Button / icon button / segmented group | `MaviButton`, `MaviButtonIcon`, `MaviButtonGroup` | buttons-actions |
| Navigation link (styled `NavLink`) | `MaviLink` | buttons-actions |
| Status badge / chip / tag | `MaviBadge`, `MaviChip` | data-display |
| SVG icon | `MaviIconMini`, `MaviIconSolid`, `MaviIconOnline` | data-display |
| Inline alert banner | `MaviAlert` | feedback-overlays |
| Toast notifications | `MaviToastr` (+ `MaviToastNotification`) | feedback-overlays |
| Modal dialog / confirm | `MaviDialog` | feedback-overlays |
| Loading overlay | `MaviBackdrop` + a spinner | feedback-overlays |
| Loading placeholder | `MaviSkeleton` | feedback-overlays |
| Loading spinner | `MaviSpinner*` (6 styles) | feedback-overlays |
| Card / titled panel | `MaviCard`, `MaviPanel` | layout-navigation |
| Expandable sections | `MaviAccordion` + `MaviAccordionItem` | layout-navigation |
| Tabs | `MaviTabNavBar` + `MaviTab` + `MaviTabContent` | layout-navigation |
| Sidebar / vertical menu | `MaviMenuVertical` | layout-navigation |
| Dropdown action menu | `MaviDropDownMenu` | layout-navigation |
| Top app bar | `TopNavBar` | layout-navigation |
| Root error boundary | `MaviErrorHandler` | layout-navigation |
| Data grid (sort/filter/page/context menus) | `MaviInMemoryTable`, `MaviCssGridTable` | tables |

## Setup (consuming project)

1. **Register services** — in `Program.cs`:
   ```csharp
   using Maviray.Blazor.Components.Core.Extensions;

   builder.Services.AddMaviComponents();
   // or configure options:
   builder.Services.AddMaviComponents(o =>
   {
       o.ComponentLogLevel = LogLevel.Warning;   // default
       o.EnableLifecycleLogging = false;         // default
       o.EnablePerformanceTracking = false;      // default
   });
   ```
   This registers `IMaviComponentOptions`. Components resolve it (and an
   `ILoggerFactory`) via DI; without registration they still render but
   lifecycle logging/options are unavailable.

2. **Include the CSS** — reference the packaged stylesheet from the host
   document (`App.razor` / `index.html` / `_Host.cshtml`):
   ```html
   <link rel="stylesheet"
         href="_content/Maviray.Blazor.Components.Material/css/maviray.material.min.css" />
   ```
   (`maviray.material.min.css` is the compiled Tailwind 4 build of the theme +
   component styles. See `material-css-styling` for the token system.)

3. **JS interop** — a few components lazy-load their own ES modules from
   `_content/Maviray.Blazor.Components.Material/js/` on first render
   (`MaviDropdown`/`MaviMultiSelect` → `maviDropDownJsInterop.js`,
   `MaviInputDate*` → `maviDatePickerJsInterop.js`). Components using
   outside-click detection (`MaviDropDownMenu`, the tables) call helpers in
   `maviJsInterop.js`; ensure that script is available to the host.
   Follow the `samples/Maviray.Blazor.Components.Samples.Material` project for
   the canonical host wiring.

4. **`@using` namespaces** — components live under
   `Maviray.Blazor.Components.Material.Components.*` (see below). Add the
   namespaces you use to the consuming project's `_Imports.razor`, e.g.:
   ```razor
   @using Maviray.Blazor.Components.Material.Components.Inputs
   @using Maviray.Blazor.Components.Material.Components.Feedback
   @using Maviray.Blazor.Components.Core.Enums
   @using Maviray.Blazor.Components.Core.Models
   ```

## Namespaces

| Area | Namespace |
|------|-----------|
| Text/number/select/date inputs, buttons, link, slider | `Maviray.Blazor.Components.Material.Components.Inputs` |
| `MaviSwitch`, `MaviToggle` | `Maviray.Blazor.Components.Material.Components` |
| `MaviBadge`, icons | `…Components.DataDisplay`, `…Components.DataDisplay.Icons` |
| `MaviChip` | `…Components.Display` |
| `MaviAlert`, `MaviBackdrop`, `MaviDialog` | `…Components.Feedback` |
| `MaviToastr`, `MaviToastNotification` | `…Components.DataDisplay` |
| `MaviAccordion`, `MaviCard`, `MaviPanel` | `…Components.Surfaces` |
| Tabs | `…Components.Tabs` |
| Menus | `…Components.Menus` |
| `TopNavBar` | `…Components.NavBars` |
| Spinners | `…Components.Spinner` |
| Tables | `…Components.Tables` |
| Enums, models, event args, attributes | `Maviray.Blazor.Components.Core.*` |

## Shared conventions (read once — they apply to most components)

### Base parameters every component has

All components derive from **`MaviComponentBase`**, which provides:

| Parameter | Type | Notes |
|-----------|------|-------|
| `Id` | `string?` | Auto-assigned GUID if unset; rendered as the element `id`. |
| `Class` | `string?` | Extra CSS classes appended to the component's computed classes — the extension seam. |
| `Style` | `string?` | Inline style. |
| `Title` | `string?` | Tooltip / accessible title (some components also render it as visible text — see `MaviButton`). |

Components deriving from **`MaviAttributesComponent`** (buttons, button group)
additionally capture unmatched attributes into `AdditionalAttributes`
(`@attributes` splat) so you can pass arbitrary HTML attributes.

### Form-input base surface

Form inputs derive from `MaviInputBase<TValue>` (which extends Blazor's
`InputBase<TValue>`, so `@bind-Value` and `EditForm`/`EditContext` validation
work) → `MaviMaterialInputBase<TValue>`. Common parameters shared by every text,
numeric, select, and date input:

| Parameter | Type | Default | Purpose |
|-----------|------|---------|---------|
| `Value` / `@bind-Value` | `TValue` | — | Two-way bound value (standard Blazor input binding). |
| `Label` | `string` | `""` | Floating label. Auto-inferred from `[Display]`/`[DisplayName]` or the property name when unset. |
| `HelperText` | `string?` | — | Hint text below the field. |
| `DisplayHelperText` | `bool` | `true` | Suppress helper text without clearing it. |
| `Required` | `bool` | `false` | Shows the asterisk; auto-inferred from `[Required]`. |
| `Disabled` | `bool` | `false` | Disables and clears validation for the field. |
| `Readonly` | `bool` | `false` | Read-only, non-editable. |
| `ThemeColorScheme` | `ThemeColorScheme` | `Primary` | Accent color (label/border/icon). |
| `ElementSize` | `ElementSize` | `Regular` | `Small` (h-10), `Regular` (h-12), `Large` (h-14). |
| `Width` | `string` | `"w-auto"` | Width utility class (e.g. `"w-96"`, `"w-full"`). |
| `StartIcon` | `string?` | — | Leading icon CSS class (e.g. a LineIcons `"lni lni-user"`). |
| `EndIcon` | `string?` | — | Trailing clickable icon. |
| `EndIconAlternative` | `string?` | — | Icon swapped in on end-icon click (toggle icons). |
| `OnEndIconClick` | `EventCallback<MouseClickEventArgs>` | — | Fires on end-icon click; shows a spinner while awaiting. |
| `EndIconDisabled` | `bool` | `false` | Disable the end-icon button only. |
| `ButtonLabel` | `string` | `"Action"` | Aria-label for the end-icon button. |
| `InferFromModelAttributes` | `bool` | `true` | Infer Label/Required/type-specific attrs from the bound property's data annotations. Set `false` to disable. |
| `Class`, `Style`, `Title`, `Id` | — | As base. |

**Attribute inference** (when `InferFromModelAttributes` is true) reads the bound
property's annotations: `[Display]`/`[DisplayName]` → `Label`, `[Required]` →
`Required`, plus per-type inference (`[MaxLength]`/`[StringLength]` →
`MaxLength`; `[DataType]` → `StringInputType`/`DateInputType`; `[Range]` →
`Min`/`Max`).

**Programmatic validation** (on any Material input, via `@ref`):
- `void SetError(string message)` — mark the field invalid with the standard
  red/alert visual state and register the message on the `EditContext` (surfaces
  in a `ValidationSummary`). Auto-clears on the next value change.
- `void ClearError()` — clear a programmatic error explicitly.

Use these inside an `OnValidSubmit` handler when a check can't be expressed as a
data annotation.

### Shared enums (`Maviray.Blazor.Components.Core.Enums`)

| Enum | Values |
|------|--------|
| `ThemeColorScheme` | `Default, Primary, Secondary, Success, Alert, Warning, Info, Dark, Light` |
| `ElementVariant` | `Filled, Outlined, Text` |
| `ElementSize` | `Regular, Large, Small` |
| `TextTransform` | `Normal, UpperCase, LowerCase, Capitalize` |
| `ButtonRole` | `Button, Submit, Reset` (maps to `type=`) |
| `IconSize` | `ExtraSmall, Small, Medium, Standard, Large, ExtraLarge, Giant` |
| `ElementColor` | 140 named CSS colors (`Red`, `DodgerBlue`, `Transparent`, …) — used by icons |
| `StringInputType` | `Text, Password, Email, Tel, Url, Search` |
| `DateInputType` | `Date, DateTime, Time, Month, Week` |
| `SkeletonVariant` | `Text, Rectangular, Rounded, Circular` |
| `SkeletonAnimation` | `None, Pulse, Wave` |
| `SortOrder` | `Ascending, Descending` |
| `BackdropOpacity` | `None, Lighten, Darken` |
| `DialogButtonClick` | `Confirm, Cancel, Close` |
| `ZIndex` | `Zero, Five, Ten, Twenty, Thirty, Forty, Fifty, Sixty, Seventy, Eighty, Ninety, OneHundred, TwoHundred … Thousand` |
| `ScreenSizeBreakpoint` | `Small … SevenExtraLarge` (Tailwind `sm`…`7xl`) |
| `ElementRelativePosition` | `CenterTop, CenterBottom, TopLeft, TopRight, BottomLeft, BottomRight` (toast position) |
| `ComponentRelativePosition` | above + `Center` (dialog position) |
| `ElementHorizontalAlignment` | `Left, Center, Right` |
| `HorizontalPosition` | `Center, Left, Right` (table cell alignment) |
| `TableColumnDataType` | `String, Boolean, Integer, Decimal, Double, Enum, DateTime, Other` |
| `TableRowContextMenuDisplayStyle` | `DropDown, Icons` |

> The component `ThemeColorScheme` enum covers a **subset** of the CSS theme
> roles — it has no `Tertiary`/`Accent`/`Highlight`. See `material-css-styling`.

### Common event payloads (`Maviray.Blazor.Components.Core`)

- `ValueChangedCallbackParameters<T>` — `{ string? ElementId; T? Value; }`.
  Many components expose a secondary `…StatusChange`/`…ValueChanged`/
  `OnSelectionChanged` callback of this type carrying the source `Id`.
- `MouseClickEventArgs` — extends `MouseEventArgs` with `ButtonId`. Emitted by
  `OnClick` on buttons, links, badges, etc.
- `MouseGroupClickEventArgs` — adds the clicked child button id (button group).
- `TabClickEventArgs`, `DialogButtonClickEventArgs`, `ButtonClickEventArgs` —
  used by tabs, dialog, and the circuit-state service respectively.

## Reference files (read the one for the component you're using)

Each file lists the components in that area with their **specific** parameters,
public methods, events, data models, and a usage snippet. The shared surface
above is not repeated there.

- **`references/inputs-forms.md`** — text, numeric, textarea, checkbox, switch,
  toggle, radio, dropdown, multi-select, autocomplete, date, slider, validation
  message.
- **`references/buttons-actions.md`** — `MaviButton`, `MaviButtonIcon`,
  `MaviButtonGroup`, `MaviLink`.
- **`references/data-display.md`** — `MaviBadge`, `MaviChip`, icon components.
- **`references/feedback-overlays.md`** — `MaviAlert`, `MaviToastr`,
  `MaviDialog`, `MaviBackdrop`, `MaviSkeleton`, spinners.
- **`references/layout-navigation.md`** — surfaces (accordion, card, panel),
  tabs, menus, top nav bar, error handler.
- **`references/tables.md`** — `MaviInMemoryTable`, `MaviCssGridTable`, and the
  table data model (`ITableDataItem`, `[TableColumn]`, `TableDataCollection`,
  `TableParameters`, context menus, events).

## Component usage pattern (the shape of almost every component)

```razor
@* Configure with strongly-typed enum parameters; extend look via Class *@
<MaviButton Title="Save"
            ThemeColorScheme="ThemeColorScheme.Primary"
            ElementVariant="ElementVariant.Filled"
            ElementSize="ElementSize.Regular"
            OnClick="HandleSaveAsync"
            Class="w-full mt-4" />

@* Inputs bind with @bind-Value and live in an EditForm for validation *@
<EditForm Model="_model" OnValidSubmit="SubmitAsync">
    <DataAnnotationsValidator />
    <MaviInputString @bind-Value="_model.Name" Width="w-96" />
    <MaviInputInteger @bind-Value="_model.Age" Min="0" Max="120" />
    <MaviButton ButtonRole="ButtonRole.Submit" Title="Submit"
                ThemeColorScheme="ThemeColorScheme.Primary" />
</EditForm>
```

**Rules of thumb**
- Configure via enum parameters; never rebuild a component's internals.
- Extend appearance via `Class` (appended to computed classes), not by forking.
- Bind data inputs with `@bind-Value`; place them in an `EditForm` for
  validation and let attribute inference fill Label/Required.
- Hold an `@ref` to call imperative methods (`Display`/`Hide` on dialogs,
  toasts, backdrops, alerts; `SetError`/`ClearError` on inputs; `Refresh` on
  tables; `Reset`/`SetSelection` on autocomplete).
- Pointers: components live in
  `src/Maviray.Blazor.Components.Material/Components/**`; enums/models in
  `src/Maviray.Blazor.Components.Core/**`; living examples in
  `samples/Maviray.Blazor.Components.Samples.Material.Client/Pages/**`.
