# Blazor UI Component Library

Collection of re-usable Blazor UI components.

## Overview
This is an attempt to centralize lots of previously relatively sparadic efforts to manage and maintain Blazor UI. Hopefully someone might find this useful. This is hardly an effort to compete or replicate effort matching one with professional UI libraries. I really like what MudBlazor folks are doing and have given it a number of serious thought inclining using those libraries. The dilemma is I am also a big fan of Tailwind idea of managing CSS with utility classes and the biggest bummer is that they or any other team i had come across don’t do Tailwind with Blazor components.

## Purpose

- Provide **simple and straightforward** library of Blazor UI components. Mostly for lazy folks who dont want to re-invent a wheel and just need something simple, working and relatively good looking.

## Structure

The repository is organized into separate projects, each representing an independent NuGet package:

```
blazor-ui-kit/
├── Maviray.Blazor.Components.Material/          # Component library following Material UI specs
├── Maviray.Blazor.Components.Tailwind/          # Component library implementing Tailwind native UI
└── ... 
```

Each project can be installed independently based on your requirements.

## Tentative Intended Content

- Inputs
  - Autocomplete
  - Button
  - Button Group
  - Checkbox
  - Radio Group
  - Rating
  - Slider
  - Switch 
  - Toggle
- Data display
  - Avatar
  - Badge
  - Chip
  - Divider
  - Icons  
  - Table
  - Tooltip
  - Typography
- Feedback
  - Alert
  - Backdrop
  - Dialog
  - Progress
  - Skeleton
  - Toastr
- Surfaces
  - Accordion  
  - Card
- Navigation
  - Side/Vertical Navigation
  - Breadcrumbs 
  - Link
  - Menu
  - Pagination
  - Stepper
  - Tabs
- Layout
  - Box
  - Container
  - Grid

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
[plugins/maviray-material](plugins/maviray-material).

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

MIT License

## Status

🚧 **Under Active Development** 🚧

This project is currently in development.  More components and documentation will be added regularly.
