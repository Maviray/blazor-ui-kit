---
name: material-css-styling
description: >-
  Use when styling, laying out, or building UI in the Maviray Blazor Material
  app — writing Tailwind classes, choosing colors/typography/spacing/elevation,
  picking or extending Mavi* components, or deciding how to implement a design.
  Applies to .razor markup, component class strings, and theme CSS. Keeps work
  on Tailwind 4 theme tokens and Material Design patterns instead of raw
  hex/stock palette colors.
---

# Material CSS Styling (Maviray Blazor)

## Overview

This app styles UI with **Tailwind 4** driven by a **Material Design** token
system. Colors, type, and elevation live as CSS variables in a `@theme` block;
components consume them through strongly-typed C# constants.

**Core principle:** Never invent a value that the theme already defines. Reach
for a component first, then Tailwind utilities bound to theme tokens — never
hardcoded hex or stock Tailwind palette colors (`blue-500`, `gray-200`, …).

## When to Use

- Building or laying out any screen, `.razor` component, or fragment
- Choosing a color, font size, spacing, border, shadow, or state style
- Deciding whether to reuse a `Mavi*` component or write custom markup
- Extending an existing component's appearance
- Editing `maviray.material.theme.css` or `Constants/Tailwind.cs`

## The Styling Decision Order

Work top-down. Only drop to the next level when the current one can't express it.

1. **Use an existing `Mavi*` component** if one fits (`MaviButton`, `MaviCard`,
   `MaviChip`, `MaviAlert`, `MaviBadge`, inputs, tabs, menus, …). Configure it
   with its enum parameters, don't rebuild it.
2. **No component fits → compose Tailwind 4 utilities** in the markup.
3. **For any themeable property (color, typography, elevation, z-index) → use a
   theme token** — a `Tailwind.Theme.*` constant in C#, or the
   `(--theme-*)` var syntax in a class string. Never a raw hex or stock palette
   color.
4. **Shape it with Material Design patterns** — the type scale, elevation
   shadows, variant × color × size structure, and hover/focus/active states.

**Violating the letter of this order (e.g. "just this one `#1976D2`") violates
its spirit — the whole point is runtime-themeable, consistent UI.**

## The Theme System (source of truth)

`src/Maviray.Blazor.Components.Material/wwwroot/css/maviray.material.theme.css`
is a Tailwind 4 `@theme` block. It is the authoritative token list — read it
rather than guessing values.

**Colors — 11 roles × 10 shades**, named `--theme-{role}-{one..ten}` where
`one` is the lightest tint and `ten` the darkest.

| Group | Roles |
|-------|-------|
| Brand / action | `primary` (blue), `secondary` (teal), `tertiary` (deep purple), `accent` (amber), `highlight` (pink) |
| Status | `success` (green), `info` (light blue), `warning` (orange), `alert` (red) |
| Neutral | `default` (gray), `light`, `dark` |

**Shade-pairing conventions** (follow these for legible contrast):

| Usage | Shade |
|-------|-------|
| Solid/filled surface bg | six–eight, with white or `light-one` text |
| Tonal/subtle surface bg (badges, hover, nav) | one–two, with `nine`/`ten` text |
| Body/label text on light bg | eight–ten |
| Borders (outlined variants) | five–seven |
| Hover on a solid surface | one step darker; on a tonal surface, one/two bg |

Exception: light fills (`warning`, `light`) use dark text (`dark-nine`/`ten`)
because the fill itself is bright — check contrast when a fill is pale.

**Other tokens** in the same file: semantic fonts `--font-ui` (Lato, body/UI),
`--font-heading` (Montserrat, titles), `--font-display` (Oswald, hero),
`--font-serif` (Roboto Serif); elevation `--mui-shadows-2/4/8`; custom
breakpoints `3xl`–`7xl`.

**Tailwind 4 var syntax** — reference a theme var directly as an arbitrary
value; add state variants as normal prefixes:

```
bg-(--theme-primary-eight)          text-(--theme-alert-nine)
border-(--theme-primary-six)        ring-(--theme-primary-seven)
hover:bg-(--theme-primary-one)      active:text-(--theme-primary-nine)
font-ui  font-heading               (font tokens work as utilities)
```

## The Tailwind Constants (`Constants/Tailwind.cs`)

In C# (`.razor`/base classes), **prefer a `Tailwind.Theme.*` constant over a
raw class string** for themeable properties. The tree:

| Namespace | Contents |
|-----------|----------|
| `Theme.Colors.{Background,Border,Text,Outline,Ring,Divide}` | Base constants `THEME_{ROLE}_{SHADE}_{PROP}`, e.g. `THEME_PRIMARY_SIX_BG`, `THEME_ALERT_NINE_TEXT` |
| …`.{Hover,Focus,Active}` (nested under each) | State variants, e.g. `THEME_PRIMARY_SIX_BG_HOVER` → `hover:bg-(--theme-primary-six)` |
| `Theme.Typography` | Material type scale (see below) |
| `Theme.Button` `.NavLink` `.Badge` `.Dialog` `.ZIndex` | Pre-composed component recipes and z-index steps |

**Constants vs. raw utilities:** use constants for color, typography, and state.
Layout/spacing/flex utilities (`flex`, `gap-2`, `p-4`, `rounded-sm`, `w-96`) stay
as plain strings — there are no constants for them and that's intended.

## Typography — Material Type Scale

Use `Tailwind.Theme.Typography.*` for semantic text instead of ad-hoc `text-*`.
Each tier bundles size + weight + tracking + leading + the right font family.

| Tier | Tokens | Font |
|------|--------|------|
| Display | `DISPLAY_LARGE/MEDIUM/SMALL` | display |
| Headline | `HEADLINE_LARGE/MEDIUM/SMALL` | heading |
| Title | `TITLE_LARGE/MEDIUM/SMALL` | heading |
| Body | `BODY_LARGE/MEDIUM/SMALL` | ui |
| Label (buttons, chips) | `LABEL_LARGE/MEDIUM/SMALL` (+`LABEL_XL`) | ui |

## Component Usage Patterns

Every `Mavi*` component takes some subset of these enums (from
`Maviray.Blazor.Components.Core.Enums`):

- **`ThemeColorScheme`**: Default, Primary, Secondary, Success, Alert, Warning,
  Info, Dark, Light. (The component enum covers a subset of the CSS roles — it
  has no Tertiary/Accent/Highlight.)
- **`ElementVariant`**: Filled, Outlined, Text.
- **`ElementSize`**: Regular, Large, Small.
- **`TextTransform`**: Normal, UpperCase, LowerCase, Capitalize (buttons default
  to UpperCase, per Material).

**Extension seam:** `MaviComponentBase` gives every component `Id`, `Class`,
`Style`, `Title`. Pass extra utilities via **`Class`** — components append it to
their computed classes, so you extend rather than fight the base. (Note: a few
components, e.g. `MaviChip`, expose `CssClass` instead of `Class` — check the
component's parameters.)

```razor
@* Configure via enums; extend via Class — don't hand-roll a button *@
<MaviButton Title="Save"
            ThemeColorScheme="ThemeColorScheme.Primary"
            ElementVariant="ElementVariant.Filled"
            ElementSize="ElementSize.Regular"
            Class="w-full mt-4" />
```

## Building Custom Markup (no component fits)

Compose in this order so the result matches the component library:

1. **Structure/layout** — `inline-flex items-center gap-2 rounded-sm …`
2. **Color** via theme vars — `bg-(--theme-primary-eight) text-(--theme-default-one)`
3. **States** — `hover:bg-(--theme-primary-nine) focus-visible:ring-2
   focus-visible:ring-(--theme-primary-seven) active:bg-(--theme-primary-four)`
4. **Typography** — a Material label/body token
5. **Elevation** — `shadow-lg` / a `--mui-shadows-*` var for raised surfaces

Include `focus-visible` rings for accessibility (the library does). If the
markup is reusable, promote it to a real component driven by the enums above,
mapping each `ThemeColorScheme` value to constants with a `switch` — mirror
`MaviButton.razor` / `MaviChip.razor`.

## Do / Don't

| Do | Don't |
|----|-------|
| `bg-(--theme-primary-eight)` / `THEME_PRIMARY_EIGHT_BG` | `bg-blue-700`, `bg-[#1976D2]`, inline `style="color:#…"` |
| Material type tokens (`BODY_MEDIUM`, `TITLE_LARGE`) for semantic text | ad-hoc `text-sm font-semibold` piles for headings/body |
| Reuse/configure a `Mavi*` component; extend via `Class` | rebuild an existing component from scratch |
| Neutral roles (`default`/`light`/`dark`) for surfaces & text | stock `gray-*`/`slate-*` for themed surfaces |
| `shadow-lg` or `--mui-shadows-*` for elevation | bespoke `box-shadow` values |
| Layout utilities (`flex`, `gap-*`, `p-*`) as plain classes | inventing constants for pure spacing/layout |

## Pointers (read these when in doubt)

- Tokens: `src/Maviray.Blazor.Components.Material/wwwroot/css/maviray.material.theme.css`
- Constants: `src/Maviray.Blazor.Components.Material/Constants/Tailwind.cs`
- Components: `src/Maviray.Blazor.Components.Material/Components/**`
- Enums: `src/Maviray.Blazor.Components.Core/Enums/**`
- Living examples: `samples/Maviray.Blazor.Components.Samples.Material.Client/Pages/**`
- CSS build: `maviray.material.index.css` imports the theme + components and is
  compiled to `maviray.material.min.css` via the `buildcss` npm script (Tailwind
  4 CLI). New `(--theme-*)` utilities you write are generated on demand from the
  markup they appear in, as long as the var exists in the theme.
