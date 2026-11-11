# Phase 3: Reorganization — Design

**Date:** 2026-11-11 · **Status:** draft for review · **Follows:** architecture review (2026-11-10), Phase 1 fixes, Phase 2 performance (`docs/superpowers/plans/2026-11-11-phase2-performance.md`)

## Intent

Phases 1 and 2 fixed bugs and cut server round-trips without changing the library's shape. Phase 3 changes the shape: it removes the copy-paste that makes every input change a 16-file edit, gives dialogs and toasts the service APIs Blazor developers expect, and cleans up the public surface so consumers need two `@using` lines instead of ten. The goal is a library that stays cheap to extend, released as **1.0.0**.

There is no user pain driving this. The library's own use so far has been light and it "works fine in general". The motivation is maintainability, consistency and size, as found by the review.

## Context

- **Consumers.** The consuming apps are the owner's own projects, which install the NuGet packages. **Breaking changes are acceptable;** the owner will update those projects. No compatibility shims or `[Obsolete]` aliases are needed.
- **Versioning.** The current release is 0.2.9. Versions come from GitVersion. A merge to `main` with `+semver: major` in the PR title or merge message releases the next major version; `+semver: minor` releases a minor one. The publish workflow packs `Core`, `Material` and `Tailwind`.
- **Codebase.**
  - .NET 10 Razor class libraries.
  - Tailwind CSS 4, built by an npm PostBuild step into `wwwroot/css/maviray.material.min.css`, which is committed.
  - bUnit/xUnit v3 test suite of 986 tests.
  - The sample apps (`samples/…Samples.Material` and `.Client`) are the living visual reference. They run their own Tailwind build for their own markup and also load the library's stylesheet.
- **Owner's role.** The owner delegated the design calls and reviews them in this document. Everything marked *(call)* is the author's decision, not something the owner asked for.

## Goals

1. One copy of the floating-label field markup, shared by every text-style input.
2. One numeric input and one date input instead of ten type-specific files, with the same behavior across types.
3. No component writes to its own `[Parameter]` properties, anywhere in the library. The one exception is the two-way bound value (`Value` through `InputBase.CurrentValue`, the editor's `Content`), which is Blazor's standard binding pattern *(call)*.
4. Dialogs and toasts usable from code through injected services.
5. Two public namespaces: `Maviray.Blazor.Components.Core` and `Maviray.Blazor.Components.Material`.
6. No global JavaScript that apps must add with a `<script>` tag, and no library CSS rules on elements the library doesn't own.
7. A build that is incremental, works offline, and treats warnings as errors.
8. A migration guide listing every breaking change, with before and after.

## Non-goals

- New components or a visual redesign.
- Retargeting to .NET 11.
- Further performance work beyond what the refactors bring; for example, table virtualization.
- Changes to the `Maviray.Blazor.Components.Tailwind` package beyond the shared build setup.

## Anti-goals (failure even if it "works")

- A visual change in any component that isn't listed in the migration guide.
- Losing an existing capability: format masks, decimal places, attribute inference, the character counter, culture handling or validation behavior.
- Breaking changes without a migration note.
- The library styling markup it doesn't render.

## Order and releases *(call)*

Five sub-projects, each with its own implementation plan, in this order:

| # | Project | Breaking | Release |
|---|---------|----------|---------|
| 1 | Housekeeping | no | can ship alone as 0.3.0 (`+semver: minor`) |
| 2 | Inputs | yes | 1.0.0 |
| 3 | Dialog and toast services | yes | 1.0.0 |
| 4 | Structure: namespaces, registration, parent/child, JS, menu, logging, dead code | yes | 1.0.0 |
| 5 | Theming and CSS | yes | 1.0.0 |

Projects 2–5 land on the feature branch one after another and ship together as 1.0.0. Project 4 renames namespaces across everything, so it runs after 2 and 3, which are the largest code moves, to avoid rebasing them over a rename. The owner merges; no project merges or pushes on its own.

Each project:
- follows test-driven development with bUnit;
- keeps the full suite green;
- updates the sample app pages it touches;
- updates the consumer docs in `.claude/skills/maviray-blazor-components/` and `.claude/skills/material-css-styling/`;
- adds its breaking changes to `docs/migration-1.0.md` *(call: one guide, grown per project)*.

---

## Project 1: Housekeeping (non-breaking)

**Build**
- Replace the PostBuild `npm install` + `npm run buildcss` targets in Material and Tailwind with an incremental target that runs before the build:
  - `Inputs`: the project's `.razor`, `.cs` and source `.css` files and `package.json`.
  - `Outputs`: the `min.css` file.
  - `npm ci` runs only when `node_modules` is missing.
  - A `SkipTailwindBuild=true` property skips the target, for offline or CI builds; the committed `min.css` is used then.
- Remove `AddRazorSupportForMvc`.

**Shared project settings**
- `Directory.Build.props` at the repo root holds what the three library csproj files duplicate today: package metadata, icon/readme/license packing, target framework, nullable, implicit usings.
- Central Package Management: `Directory.Packages.props` with today's versions.

**Analyzers**
- Enable the .NET analyzers (`AnalysisLevel` latest, default mode) for `src/` projects.
- Fix the warnings that surface.
- Then set `TreatWarningsAsErrors` for `src/`. Tests and samples keep warnings as warnings *(call)*.

**Trimming**
- Mark Core and Material `IsTrimmable`.
- Fix the trim warnings, for example a `DynamicallyAccessedMembers` annotation on `TableDataCollectionFactory.FromDataSource<T>` and the enum display-name lookup.

**npm**
- Update the Tailwind packages to the latest 4.x.
- The regenerated `min.css` must still contain every class it had; confirm by diffing the selector lists.

**Logging quick fix**
- `LogDebugLifeCycle` logs at Debug instead of Information.
- Remove AutoComplete's unconditional Information log.

The full logging redesign is in Project 4.

---

## Project 2: Inputs

**Components after this project**

| Component | Replaces | Value types |
|-----------|----------|-------------|
| `MaviInputString` | `MaviInputString`, `MaviInputStringNullable` | `string?` |
| `MaviTextArea` | same | `string?` |
| `MaviInputNumber<TValue>` | `MaviInputShort`, `…Integer`, `…IntegerNullable`, `…Long`, `…Float`, `…Double`, `…DoubleNullable`, `…Decimal`, and `MaviNumericInputBase` | `short`, `int`, `long`, `float`, `double`, `decimal` and their nullables |
| `MaviInputDate<TValue>` | `MaviInputDate`, `MaviInputDateNullable` | `DateTime`, `DateTime?` |
| `MaviDropdown`, `MaviMultiSelect`, `MaviAutoComplete` | unchanged | — |

- `TValue` is inferred from `@bind-Value`, so markup reads `<MaviInputNumber @bind-Value="_model.Age" />`. An unsupported `TValue` throws on first render with a clear message.
- `MaviInputString` gains `ConvertEmptyToNull` (default false). It replaces `MaviInputStringNullable`'s whitespace-to-null behavior.

**One copy of the field markup**
- The container, theme variables, label, required asterisk, start icon, end icon or spinner, fieldset/legend and helper text are written once, in the input base.
- Each input supplies only its control element (input, textarea, or trigger div), which keeps the `peer` sibling order Phase 2's CSS relies on.
- The mechanism is the builder's choice: a protected `RenderFragment` helper written in Razor on the base, or an internal component. The acceptance criterion: no field markup appears in more than one file, and the rendered DOM of every input matches today's, element for element and class for class.

**Unified numeric behavior** *(call)*
- **Range and step.**
  - `Min`, `Max` and `Step` are `decimal?` for every type. That is exact for the integer types and `decimal`, and covers ±7.9 × 10²⁸ for `float`/`double`.
  - `Step` defaults to 1 for integer types and is ignored for fractional types, which render as text inputs.
  - Values outside `Min`/`Max` produce a validation error on commit, for every type. Today only `short` does this.
  - `[Range]` inference applies to every type.
- **Fractional types only.**
  - `DecimalPlaces` rounds on commit (away from zero) and fixes the displayed decimals.
  - `AllowScientificNotation` applies to `float`/`double`.
- **Empty versus zero.** A non-nullable number shows `0` when its value is 0. Today it shows an empty field, so 0 can't be told apart from "nothing". Nullable numbers show empty for `null`.
- **Input element.**
  - Integer types use a native number input when there's no `FormatMask`, or when the mask is numeric (N/D/G/F), the field is focused, and `ShowFormattedWhileFocused` is off.
  - Otherwise they use a text input.
  - Fractional types always use a text input with `inputmode="decimal"`.
- **Culture.** Phase 1's rules stay: `FormatCulture`, otherwise `CurrentCulture`; native number inputs always use the invariant culture.
- **Commit.** Phase 1 and 2's commit and validation-timing rules stay unchanged: bind on change, the `Immediate` opt-in, validate once per commit.

**No component writes its own parameters**
- Attribute inference and defaults go into private fields read through `Effective*` properties, for example:
  - `EffectiveLabel`, `EffectiveRequired`
  - numeric `EffectiveMin` / `EffectiveMax`
  - string `EffectiveMaxLength` / `EffectiveInputType`
  - Dropdown enum `EffectiveItems`
  - AutoComplete's default end icon and button label
  - the end-icon/alternative-icon swap on click, which becomes private toggle state
- A parent re-render then never overwrites or loses an inferred value.
- Attribute lookups are cached per `PropertyInfo` in a static thread-safe cache.

**Phase 2 follow-ups folded in**
- All field class builders are allocation-light: no `List<string>`; this includes the TextArea, Dropdown and MultiSelect triggers.
- `FieldThemeStyle` comes from a static per-scheme table.
- `InputAttributes` is rebuilt only when `AdditionalAttributes` or `BindOnInput` change.
- The commit helper mirrors `InputBase`'s "nullable and empty text" rule instead of parsing twice.
- `HasError` is computed once per render.
- An app-supplied `onchange` or `oninput` passed through attributes no longer replaces the field's own handler. The field handles the event first, then invokes the app's handler *(call)*.
- Dropdown and AutoComplete selection validates once, through the commit helper.
- Phase 2's test gaps are covered: parse-failure and nullable-empty commits, MultiSelect legend, Dropdown blur, counter text, attribute handler precedence.

---

## Project 3: Dialog and toast services

**Toasts**
- `IToastService` (scoped):
  - `Show(MaviToastNotification toast)` passes the record through unchanged. As today, `Duration = null` means the toast stays until closed.
  - `Show(string message, string? title = null, ThemeColorScheme scheme = ThemeColorScheme.Default, TimeSpan? duration = null)` *(call)*:
    - omitting `duration` (`null`) uses a 5-second default, configurable when registering the services;
    - `Timeout.InfiniteTimeSpan` makes the toast stay until closed (stored as `Duration = null`).
  - `Dismiss(string id)` and `Clear()`
- `MaviToastr` keeps today's duration rules.
- `<MaviToastProvider Position MaxVisible NewestOnTop />` is placed once in the layout. It renders the service's toasts through the existing `MaviToastr` and handles closing and auto-dismiss.
- `MaviToastr` stays public as the presentational component.

**Dialogs**
- `IDialogService` (scoped):
  - `Task<DialogButtonClick> ShowAsync(string title, RenderFragment body, MaviDialogOptions? options = null)`
  - `Task<bool> ConfirmAsync(string title, string message, MaviDialogOptions? options = null)`
  - Closing through the backdrop or the close button completes the task with `DialogButtonClick.Close`.
- `<MaviDialogProvider />` is placed once in the layout and renders the open dialogs with the existing dialog visuals. Dialogs opened while another is open stack, newest on top *(call)*.
- `MaviDialogBaseParameters` is renamed `MaviDialogOptions` and becomes an immutable record, with the same properties and defaults.
- **The busy-then-close flow stays** *(call)*. Today the dialog shows a spinner while the app's click handler runs, and `CloseOnUserAction = false` keeps it open after Confirm. Both work from the service:
  - `MaviDialogOptions` gains `Func<DialogButtonClick, Task<bool>>? OnButtonClick`.
  - The dialog shows its busy spinner while the callback runs. The callback returns `true` to close or `false` to stay open.
  - Without a callback, `CloseOnUserAction` decides as today.
  - The `ShowAsync` / `ConfirmAsync` task completes when the dialog closes, with the button that closed it.
- The inline `<MaviDialog>` stays for custom layouts:
  - Its `Display(...)` methods return `Task<DialogButtonClick>` with the same rule: the task completes when the dialog closes. Callers that `await` it today will wait for the user (migration note).
  - `SetBusy`, `SetIdle` and `Hide` stay on the inline component.
  - It never mutates an options object passed to it, and never writes its `Title` parameter. Today `Display(parameters)` changes the parent's object, and `Display(string title)` overwrites `Title`.

**Missing provider.** Calling either service when no provider is rendered throws an `InvalidOperationException` that names the provider to add.

**Render mode.** The owner's apps are interactive app-wide, like the sample with `InteractiveWebAssembly` on `<Routes>`. The providers therefore go in the layout and share the pages' DI scope. The consumer docs state that requirement. Per-page interactivity is not supported in 1.0 *(owner's answer)*.

---

## Project 4: Structure

**Namespaces and registration**
- All public Core types live in `Maviray.Blazor.Components.Core`, and all public Material types in `Maviray.Blazor.Components.Material`. Internal types may stay in sub-namespaces.
- Consumers write two `@using` lines. `MenuItem` and `MenuItemGroup`, which have no namespace today, get the Material one.
- `services.AddMaviComponents()` moves to Material and registers everything Material needs, including Project 3's services. Core's registration becomes internal *(call)*.

**Parent/child components**
- `MaviTabNavBar`/`MaviTab`/`MaviTabContent` and `MaviRadioGroup`/`MaviRadioGroupItem` use `MaviAccordion`'s pattern:
  - The parent cascades itself with `IsFixed="true"`.
  - Children register in `OnInitialized` and unregister in `Dispose`.
  - The parent tells only the affected children to re-render.
- `MaviTabContext` and `RadioGroupContext` are removed.

**Menu**
- `MaviMenuVertical` gets a two-way bindable `Expanded` (`@bind-Expanded`) instead of listening to `ICircuitStateService.ButtonClicked`.
- Removed: `ICircuitStateService`, `CircuitStateService`, `MavirayLayoutBase`, `ElementConstants`, and `ButtonClickEventArgs` if it becomes unused. The sample's layout binds `Expanded` directly.

**JavaScript**
- `maviJsInterop.js` (`window.registerGlobalOutsideClickListener`) becomes an ES module loaded on demand, and the sample's `<script>` tag is removed.
- One internal scoped service imports each JS module once per circuit and caches the import. Dropdown, MultiSelect, date, tables and the editor use it.
- One shared disposal helper ignores `JSDisconnectedException`.

**Logging**
- Remove `EnableLifecycleLogging`, `IMaviComponentOptions`/`MaviComponentOptions` and the custom `LoggerExtensions`, together with the lifecycle trace calls (75 call sites).
- Errors and warnings stay, written as source-generated `[LoggerMessage]` methods.
- Apps control verbosity with the standard `Logging:LogLevel:Maviray.Blazor.Components` filter *(call)*.

**Components that still write their own parameters** (Goal 3; inputs are in Project 2, dialogs in Project 3)
- `MaviAlert.Display(...)` keeps its signature but stores the text, title, variant and scheme it receives as private display state instead of overwriting the `Text`, `Title`, `ElementVariant` and `ThemeColorScheme` parameters.
- `MaviInMemoryTableBase` reads `EffectiveParameters` (the parameter, or defaults) instead of assigning `Parameters ??= new()`.

**Dead code and smaller API cleanups**
- Remove `JsInteropConstants` (after the JS change) and any constant classes left unused.
- `MaviAttributesComponent.AdditionalAttributes` becomes `IReadOnlyDictionary<string, object>?`.
- `GetAccentTextColor` becomes private.
- Fix the stale comments Phase 2's review listed.

---

## Project 5: Theming and CSS

**Component colors through CSS variables**
- Phase 2's `FieldThemeStyle` approach is generalized: each color scheme sets a small set of CSS custom properties once, and components use fixed classes that read them.
- This replaces the 79 `ThemeColorScheme` switch blocks in component code.
- The variable names and the exact set per scheme are the builder's choice, as long as every scheme × variant × state renders today's colors.

**No global element styling**
- The library stylesheet stops styling elements it doesn't render:
  - the `[type='text']` … `[multiple]` `@apply` block in `maviray.material.components.css`;
  - the `ul`/`ol` base rule;
  - the `h1`–`h6` focus rule.
- Components that relied on these get the classes explicitly.
- The migration guide tells apps which rules to add to their own CSS if they want them back.

**Plugins**
- Remove `@tailwindcss/typography`; nothing in the library uses `prose`.
- Switch `@tailwindcss/forms` to `strategy: "class"` *(call)*. In its default mode it restyles every `input`, `select` and `textarea` on the page, which Goal 6 rules out.
  - Components that render native checkboxes, radios or selects get the plugin's `form-*` classes, or explicit utilities, so they keep today's look.
  - The owner's visual checklist covers checkbox, radio, switch, toggle, slider and dropdown in every state.

**Trim the `Tailwind.Theme` constants and the stylesheet** (owner's decision)

Background:
- Today `Constants/Tailwind.cs` has 3,130 constants. Components use about 266, and the sample uses many more.
- Because the class strings live in the library, an app's own Tailwind build never sees them; they work only because the library stylesheet ships all of them. That is most of the 318 KB.

The owner's apps barely use the constants, so:
- **Typography stays public.** `Tailwind.Theme.Typography` remains the public type-scale API, and its classes stay in the library stylesheet, because an app's Tailwind build can't generate classes whose strings live in the library.
- **Everything else becomes internal or goes.** The color, badge, nav-link, button and dialog constants become internal, keeping only those the components use. After Project 5's CSS-variable work, most component uses disappear too.
- **Apps style colors with `(--theme-*)` classes.** For example `text-(--theme-primary-eight)` in their own markup, generated by their own Tailwind build. The `--theme-*` variables are defined at runtime by the library stylesheet, so the app's build needs no access to the library's theme file. The migration guide shows the before/after for each constant group.
- **The sample pages follow.** They switch from color constants to `(--theme-*)` classes; the sample already runs its own Tailwind build.
- **Acceptance:** every class in `min.css` is referenced by library source or by the Typography constants, and the migration guide states the old and new stylesheet size.

---

## Testing and verification

- **Tests first.** Every behavior change gets a failing bUnit test first.
- **Refactors keep rendering identical.** Pure refactors (field markup, namespace moves, theming variables) are guarded by characterization tests written before the change, which assert the rendered markup and classes of representative components across sizes, schemes and states.
- **CSS checks.** Generated CSS is checked by grepping `min.css` for new classes, as in Phase 2.
- **Owner's visual check.** Projects 2 and 5 end with a visual check by the owner in the sample app, from a checklist the plan provides. The author can't see a browser in these sessions.

## Left to the builder

- File layout and the Razor mechanism for the shared field markup.
- Internal class and helper names.
- How each numeric type parses and formats without per-call reflection: generic math, or a switch on the type.
- Validation message wording, as long as it's consistent across types.
- The CSS variable naming scheme.
- The internal JS module service's shape.
- Test names.

## Calls made by the author (for review)

1. Release plan: Project 1 can ship alone as 0.3.0; Projects 2–5 ship together as 1.0.0.
2. No compatibility shims; one migration guide at `docs/migration-1.0.md`.
3. Merged components drop the old type-specific names. Generic `TValue` is inferred from `@bind-Value`.
4. `Min`/`Max`/`Step` are `decimal?`; out-of-range values fail validation for every numeric type.
5. A non-nullable number shows `0` instead of blank.
6. Date inputs cover `DateTime` and `DateTime?` only; no `DateOnly`.
7. App-supplied `onchange`/`oninput` handlers run after the field's own.
8. Stacked dialogs. Toasts shown from a message default to 5 seconds; `Timeout.InfiniteTimeSpan` keeps them until closed.
9. `AddMaviComponents()` moves to Material.
10. Lifecycle trace logging is removed rather than redesigned.
11. `TreatWarningsAsErrors` for `src/` only.
12. `@tailwindcss/forms` switches to class strategy; `@tailwindcss/typography` goes.
13. Dialogs keep busy-then-close through an `OnButtonClick` callback returning close/stay. Awaiting a dialog completes when it closes.
14. Goal 3 covers the whole library; the two-way bound value is the only exception.

## Owner's answers

1. **`Tailwind.Theme` constants:** the owner's apps barely use them. Trim them, keep Typography public, and have apps write `(--theme-*)` classes (Project 5).
2. **Render mode:** the owner's apps are interactive app-wide. Toast and dialog providers go in the layout (Project 3).
