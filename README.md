# Blazor UI Component Library

Collection of re-usable Blazor UI components.

## Overview
The idea is to compile simple collection of re-usable Blazor UI components.

## Purpose

- Provide **simple and straightforward** Blazor UI components

## Structure

The repository is organized into separate projects, each representing an independent NuGet package:

```
blazorui/
├── Project1/          # Component library project 1
├── Project2/          # Component library project 2
└── ... 
```

Each project can be installed independently based on your requirements.

## Tentative Intended Content

- Inputs & Controls
  - Button (variants: filled, tonal, outlined, text, icon-only; states & loading)
  - IconButton
  - FAB (small/regular/large)
  - ToggleButton / ToggleButtonGroup
  - Checkbox
  - Radio
  - Switch
  - Slider (single/range, tick marks)
  - TextField (filled/outlined; with leading/trailing icons; validation, helper, counter)
  - TextArea
  - Select (native & custom popover)
  - Autocomplete / Combobox
  - DatePicker / DateRangePicker
  - TimePicker
  - Chip (assist/filter/input/suggestion) + ChipSet
  - SegmentedButton
  - Stepper (horizontal/vertical)
- Data Display
  - Badge
  - Avatar
  - Icon (Material Symbols)
  - List / ListItem (one-line/two-line/three-line, leading/trailing actions)
  - Table (sortable, pageable, density, sticky header/cols, row selection)
  - DataGrid-lite (virtualized rows, selection, multi-sort if feasible)
  - Tooltip
  - Divider
- Feedback & Surfaces
  - Snackbar / Toast (stacking, action button, dismiss, durations)
  - Dialog (modal, full-screen on small screens)
  - Drawer (modal, dismissible, permanent)
  - AppBar / TopAppBar (small/centered/large)
  - Card (elevated, filled, outlined)
  - Banner
- Navigation
  - Tabs (primary/secondary, fixed/scrollable)
  - NavigationRail
  - NavigationDrawer
  - BottomNavigation
  - Breadcrumbs
  - Pagination
- Layout
  - Grid helpers (CSS Grid utilities, responsive)
  - Stack/Spacer helpers
  - Responsive container (breakpoints aligned to Material 3)
  - Surface container tokens for elevation overlays
- Utility Behaviors
  - FocusTrap
  - ScrollLock
  - Ripple (JS interop or CSS-only approximation)
  - Elevation & state layers (hover/pressed/disabled)
  - Motion helpers (CSS transitions aligned to Material durations/easings)

## Installation

*(Coming soon)*

Install packages via NuGet Package Manager: 

```bash
dotnet add package [PackageName]
```

## Usage

*(Coming soon)*

Example of using a component: 

```razor
@using [Namespace]

<ComponentName />
```

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

MIT License

## Status

🚧 **Under Active Development** 🚧

This project is currently in development.  More components and documentation will be added regularly.
