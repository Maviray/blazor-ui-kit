# Phase 2 Performance Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Cut server round-trips, re-renders and per-render allocations in `Maviray.Blazor.Components.Material` without changing how components look.

**Architecture:** Text inputs stop sending keystroke and focus events to .NET: values bind on `change` (opt back in with `Immediate`), and the floating label and focus colors move to Tailwind `peer-*` variants driven by CSS custom properties set once per theme. Hot render paths (class building, icon lookup, table cell lookup and sorting) drop reflection, LINQ closures and throwaway collections.

**Tech Stack:** .NET 10 / C# 14, Blazor components, Tailwind CSS 4.1 (built by the project's PostBuild `npm run buildcss`), bUnit 2.15, xUnit v3, AwesomeAssertions.

**Spec:** Architecture review of 2026-11-10 (this conversation), "Performance" section and "Suggested order → Phase 2". User decisions of 2026-11-11: inputs update on change by default with an `Immediate` opt-in; do the CSS-only label now and the user verifies visuals in the sample app.

## Global Constraints

- Target framework stays `net10.0`; no new NuGet packages.
- Tailwind only generates classes that appear as complete literals in `.cs`/`.razor` source. Never build a utility class by concatenating or interpolating its parts.
- No visual change intended. Theme colors, sizes and positions must match the current C# output state by state.
- Public API changes allowed only as listed: new `Immediate` parameter, binding default (change instead of input), removal of `protected bool IsLabelFloating`, new `CssManager.Combine(params ReadOnlySpan<string?>)` overload.
- Test command: `dotnet test --project tests/Maviray.Blazor.Components.Material.Tests/Maviray.Blazor.Components.Material.Tests.csproj -- --filter-class "*<ClassName>"`; full suite without the filter.
- One commit per task on `feature/ongoing-development`, ending with the `Co-Authored-By` trailer.

## Review Focus

- A TextArea with `ShowCharacterCount` must keep counting while the user types, even though binding now defaults to change (pinned in Task 1).
- A field with a validation error stays error-colored while focused; focus styling must not override it (pinned in Task 2).
- An open Dropdown with no selection keeps its label floated even after focus moves into the list (pinned in Task 2).
- MultiSelect's trigger is a `div`, which never matches `:placeholder-shown`; the "has value" CSS rule must not be applied to it or its label floats permanently (pinned in Task 2).
- Table sorting with missing cells, null values and values outside `int` range must order correctly instead of treating them as missing (pinned in Task 5).

---

### Task 1: Inputs bind on change by default (`Immediate` opt-in)

**Files:**
- Modify: `src/Maviray.Blazor.Components.Material/Components/Inputs/MaviMaterialInputBase.cs`
- Modify (markup only): `MaviInputString.razor`, `MaviInputStringNullable.razor`, `MaviTextArea.razor`, `MaviInputInteger.razor`, `MaviInputIntegerNullable.razor`, `MaviInputLong.razor`, `MaviInputShort.razor`, `MaviInputFloat.razor`, `MaviInputDouble.razor`, `MaviInputDoubleNullable.razor`, `MaviInputDecimal.razor`, `MaviInputDate.razor`, `MaviInputDateNullable.razor` (all in `Components/Inputs/`)
- Test: `tests/.../Components/Inputs/InputBindingTests.cs` (new); update `InputValidationTimingTests.cs`

**Interfaces:**
- Produces on `MaviMaterialInputBase<TValue>`:
  - `[Parameter] public bool Immediate { get; set; }`: when true, the bound value updates on every keystroke.
  - `protected virtual bool BindOnInput => Immediate;`
  - `protected IReadOnlyDictionary<string, object>? InputAttributes`: `AdditionalAttributes`, plus `"oninput" → EventCallback.Factory.Create<ChangeEventArgs>(this, HandleInput)` when `BindOnInput`.
- `MaviTextArea`: `protected override bool BindOnInput => Immediate || ShowCharacterCount;`
- Markup: remove `@oninput="HandleInput"` and replace `@attributes="AdditionalAttributes"` with `@attributes="InputAttributes"` on the input/textarea element. `MaviAutoComplete` is unchanged, because search needs keystrokes.

Why a dictionary: `@oninput="@(cond ? HandleInput : null)"` still attaches a handler, because Razor wraps it in an `EventCallback` whose receiver is the component. Omitting the attribute is the only way to stop the browser sending the event.

- [ ] **Step 1: Write failing tests** in `InputBindingTests`:
  - `StringInput_Typing_IsNotSentToTheComponent`: `Action typing = () => cut.Find("input").Input("ab"); typing.Should().Throw<MissingEventHandlerException>();`
  - `IntegerInput_Typing_IsNotSentToTheComponent`: same for `MaviInputInteger`.
  - `StringInput_Change_UpdatesBoundValue`: `Change("ab")` → bound value `"ab"`.
  - `ImmediateStringInput_Typing_UpdatesBoundValue`: `Immediate = true`, `Input("ab")` → bound value `"ab"`.
  - `TextAreaWithCharacterCount_UpdatesWhileTyping`: `ShowCharacterCount = true`, `Input("abc")` → bound value `"abc"`.
  - In `InputValidationTimingTests`, add `.Add(x => x.Immediate, true)` to the two `*_Typing_*` tests. They test typing without validation, which now only applies to immediate inputs.
- [ ] **Step 2: Run** `--filter-class "*InputBindingTests"`. Expected: the two `*_IsNotSentToTheComponent` tests fail (no exception thrown); the others pass.
- [ ] **Step 3: Implement** the interfaces above and the markup edits in the 13 files.
- [ ] **Step 4: Run the full suite.** Expected: all pass.
- [ ] **Step 5: Commit** `perf: bind inputs on change by default; add Immediate opt-in`.

### Task 2: Floating label and focus styling in CSS

**Files:**
- Modify: `Components/Inputs/MaviMaterialInputBase.cs` (builders and theme)
- Modify: `MaviDropdown.razor`, `MaviMultiSelect.razor` (overrides and their own trigger builders), `MaviTextArea.razor` (`BuildTextAreaClass`), `MaviAutoComplete.razor`
- Modify (markup): the 16 inputs that render `BuildFieldsetClass()`
- Test: `tests/.../Components/Inputs/FieldStylingTests.cs` (new)

**Interfaces (on `MaviMaterialInputBase<TValue>`):**
- `protected string FieldThemeStyle { get; private set; }`, set in `SetThemeColors()`, for example for Primary:
  `--mavi-field-text: var(--theme-primary-eight); --mavi-field-text-focus: var(--theme-primary-nine); --mavi-field-border: var(--theme-primary-eight); --mavi-field-border-hover: var(--theme-primary-nine); --mavi-field-border-focus: var(--theme-primary-nine);`
  For `ThemeColorScheme.Light` the three border variables are `--theme-light-three` (rest), `--theme-light-five` (hover) and `--theme-light-seven` (focus), matching today's `SetThemeColors`.
- `protected virtual bool ForceLabelFloating => Disabled || Readonly;`
  - Dropdown and MultiSelect override with `base.ForceLabelFloating || _isOpen || HasValue`.
- `protected virtual bool ShowActiveState => false;`: a server-known "focused" look.
  - Dropdown and MultiSelect override with `_isOpen || IsFocused`.
- `protected virtual bool FloatsLabelOnValue => true;`
  - MultiSelect overrides with `false`.
- Remove `IsLabelFloating`. Keep `IsFocused`, `OnFocus` and `OnBlur`; numeric, date, Dropdown and MultiSelect logic still uses them.

**Class rules.** Every string below must appear literally in source.

The input element's class includes `peer`. This applies to `BuildInputClass`, `BuildTextAreaClass`, `BuildDropdownClass` and the MultiSelect trigger.

**Label**
- Always: `absolute transition-all duration-200 pointer-events-none leading-none -translate-y-1/2`.
- Forced floating: `top-[4px] left-[11px] px-1 text-xs`, or `top-[5px]` instead of `top-[4px]` when Disabled or Readonly.
- Otherwise: the resting size classes from today's `BuildLabelClass`, plus:
  - `peer-focus:top-[4px] peer-focus:left-[11px] peer-focus:px-1 peer-focus:text-xs`
  - when `FloatsLabelOnValue`, also `peer-not-placeholder-shown:top-[4px] peer-not-placeholder-shown:left-[11px] peer-not-placeholder-shown:px-1 peer-not-placeholder-shown:text-xs`
- Color (also for the start icon, end icon and end-icon spinner):
  - `HasError`: the alert text color only.
  - else `ShowActiveState`: `text-(--mavi-field-text-focus)`.
  - else Disabled or Readonly: today's black/opacity colors.
  - else `text-(--mavi-field-text) peer-focus:text-(--mavi-field-text-focus)`.

**Fieldset**
- `HasError`: `border-2` plus the alert border color.
- else `ShowActiveState`: `border-2 border-(--mavi-field-border-focus)`.
- else Disabled or Readonly: unchanged from today.
- else `border-(--mavi-field-border) hover:border-(--mavi-field-border-hover) peer-focus:border-2 peer-focus:border-(--mavi-field-border-focus)`.

**Legend**
- `max-w-0` unless forced floating, plus:
  - `[.peer:focus~fieldset_&]:max-w-full`
  - when `FloatsLabelOnValue`, also `[.peer:not(:placeholder-shown)~fieldset_&]:max-w-full`
- Forced floating: `max-w-full`.

**Markup (16 files)**
- Add `style="@FieldThemeStyle"` to the `GetContainerClass()` div.
- Add `placeholder=" "` to the input/textarea (not MultiSelect).
- Remove `@onfocus="OnFocus"` from String, StringNullable, TextArea and AutoComplete.
- Remove `@onblur="OnBlur"` from String, StringNullable and TextArea.

Write builders with `string.Concat`/interpolation of these literals; no `List<string>`.

- [ ] **Step 1: Write failing tests** in `FieldStylingTests`:
  - `TextInputs_DoNotRoundTripOnFocus`, a Theory over String, StringNullable and TextArea: `Focus()` throws `MissingEventHandlerException`.
  - `Label_FloatsThroughCss`: the String input's element has the class `peer` and the attribute `placeholder=" "`; the label class contains `peer-focus:top-[4px]` and `peer-not-placeholder-shown:top-[4px]`.
  - `DisabledLabel_AlwaysFloats`: the label class contains ` top-[5px] ` and no `peer-focus:top-[4px]`.
  - `ErrorColor_IsNotOverriddenByFocus`: after `SetError("x")` inside an `EditContext`, the label class contains `text-(--theme-alert-eight)` and no `peer-focus:text-`.
  - `OpenDropdown_FloatsLabelWithoutValue`: after a click on the Dropdown input, the label class contains `top-[4px]` with no `peer-` prefix.
  - `MultiSelectLabel_DoesNotUseThePlaceholderRule`: the label class contains no `placeholder-shown`.
  - `ThemeColors_AreCssVariables`: with `ThemeColorScheme.Success`, the container `style` contains `--mavi-field-text-focus: var(--theme-success-nine)`; with `Light` it contains `--mavi-field-border: var(--theme-light-three)`.
- [ ] **Step 2: Run** `--filter-class "*FieldStylingTests"`. Expected: all fail.
- [ ] **Step 3: Implement** the interfaces, class rules and markup edits.
- [ ] **Step 4: Run the full suite.** Expected: all pass. Update any older test that asserted the replaced classes.
- [ ] **Step 5: Verify the generated CSS.** Run `dotnet build src/Maviray.Blazor.Components.Material` (PostBuild runs Tailwind), then `grep -o` in `wwwroot/css/maviray.material.min.css` for each selector:
  - `peer-not-placeholder-shown\:top-\[4px\]`
  - `peer-focus\:border-\(--mavi-field-border-focus\)`
  - `\[\.peer\:focus\~fieldset_\&\]\:max-w-full`
  - `\[\.peer\:not\(\:placeholder-shown\)\~fieldset_\&\]\:max-w-full`
  - `text-\(--mavi-field-text-focus\)`

  Expected: each is found. If `peer-not-placeholder-shown` is missing, switch to `peer-[:not(:placeholder-shown)]:` (in source and tests) and re-check.
- [ ] **Step 6: Commit** `perf: float labels and show focus with CSS instead of server round-trips`.
- [ ] **Step 7: Hand to the user for a visual check** in the sample app, for each of String, Integer, TextArea, Date, Dropdown and MultiSelect:
  - empty and unfocused → label rests inside the field
  - focused → label floats, border turns 2px and uses the accent color
  - typed value, then blurred → label stays floated
  - invalid → red, also while focused
  - disabled and readonly
  - Light theme scheme

### Task 3: Lighter class building

**Files:**
- Modify: `src/Maviray.Blazor.Components.Core/Utilities/CssManager.cs`
- Modify: `Components/Inputs/MaviDropdown.razor`, `Components/Inputs/MaviMultiSelect.razor`
- Modify: `src/Maviray.Blazor.Components.Material/Maviray.Blazor.Components.Material.csproj`: add `<InternalsVisibleTo Include="Maviray.Blazor.Components.Material.Tests" />`, used by Tasks 4–5.
- Test: `tests/.../Utilities/CssManagerTests.cs` (new), `tests/.../Components/Inputs/SelectionListTests.cs`

**Interfaces:**
- `public static string Combine(params ReadOnlySpan<string?> classes)`. The existing `Combine(params string?[]? classes)` delegates to it. Same contract: split on space, tab, CR and LF; trim; drop empties; keep the first occurrence (ordinal); join with one space. Tokenize with `MemoryExtensions.Split`/`SplitAny` over spans, write into an `ArrayPool<char>` buffer, dedupe by scanning the buffer, and allocate only the result string.
- Dropdown/MultiSelect: `GetOptionClass(bool isSelected)` returns two strings computed in `OnParametersSet` (after `base.OnParametersSet()`), not rebuilt per option.

- [ ] **Step 1: Write tests** in `CssManagerTests`:
  - `Combine_SplitsTrimsAndDedupes`: `Combine("a  b", " b c\t", null, "", "a\nd")` == `"a b c d"`.
  - `Combine_WithNoClasses_ReturnsEmpty`: `Combine()` and `Combine(null, " ")` == `""`.
  - `Combine_AllocatesOnlyTheResult`: warm up, then measure `GC.GetAllocatedBytesForCurrentThread()` around `CssManager.Combine("px-2 py-1", "text-sm", "px-2")`; expected `< 120` bytes.

  In `SelectionListTests`, add `Dropdown_OptionClasses_AreUnchanged`, asserting the literal class strings today's `GetOptionClass` emits for a selected and an unselected option (Regular size, Primary).
- [ ] **Step 2: Run** `--filter-class "*CssManagerTests"`. Expected: only `Combine_AllocatesOnlyTheResult` fails.
- [ ] **Step 3: Implement** the interfaces above.
- [ ] **Step 4: Run the full suite.** Expected: all pass.
- [ ] **Step 5: Commit** `perf: allocation-light CssManager.Combine and cached option classes`.

### Task 4: Icon lookup without reflection

**Files:**
- Modify: `Components/DataDisplay/Icons/MaviIconMiniPaths.cs`, `MaviIconOnlinePaths.cs`, `MaviIconSolidPaths.cs`
- Test: `tests/.../Components/DataDisplay/IconTests.cs` (new)

**Interfaces:**
- In each file, every `public static List<string> X => [ ... ];` becomes `private static readonly string[] X = [ ... ];`.
- `GetIconPaths(IconXType iconType)` becomes a `switch` expression with one arm per enum member (`IconMiniType.AcademicCapMini => AcademicCapMini,` …, `_ => []`). Its return type stays `IEnumerable<string>`.
- Generate the arms with a throwaway script in the scratchpad. Names match the enum case-insensitively; all 287 / 286 / 286 members have a path set.

- [ ] **Step 1: Write tests:**
  - `EveryMiniIcon_RendersPaths`, `EveryOnlineIcon_RendersPaths`, `EverySolidIcon_RendersPaths`: Theories over `Enum.GetValues<...>()`; each renders its component and finds at least one `path` element.
  - `IconPaths_AreSharedBetweenLookups`: `ReferenceEquals(MaviIconMiniPaths.GetIconPaths(IconMiniType.AcademicCapMini), MaviIconMiniPaths.GetIconPaths(IconMiniType.AcademicCapMini))` is true.
- [ ] **Step 2: Run** `--filter-class "*IconTests"`. Expected: only `IconPaths_AreSharedBetweenLookups` fails.
- [ ] **Step 3: Implement** the conversion in all three files.
- [ ] **Step 4: Run the full suite.** Expected: all pass.
- [ ] **Step 5: Commit** `perf: look up icon paths with a switch instead of reflection`.

### Task 5: Table cell lookup, typed sorting, menus on demand

**Files:**
- Modify (Core): `Models/Tables/MaviTableRow.cs`, `MaviTableCell.cs`, `MaviTableSorter.cs`, `TableDataCollection.cs`, `TableDataCollectionFactory.cs`
- Modify (Material): `Components/Tables/MaviInMemoryTable.razor`, `MaviCssGridTable.razor`
- Test: `tests/.../Models/Tables/TableSortingTests.cs` (new), `tests/.../Components/Tables/MaviInMemoryTableTests.cs`

**Interfaces:**
- `MaviTableRow`: `public MaviTableCell? GetCell(string columnKey)`, a plain loop with ordinal key comparison and no LINQ. `GetCellValue` uses it.
- `MaviTableSorter` additionally implements `IComparer<MaviTableCell?>`. The rules:
  - Missing cell or null `OriginalValue` sorts after any value.
  - If both values have the same runtime type, the type implements `IComparable`, and the column type is not `String` or `Other`: compare with `CompareTo`.
  - Otherwise: culture string compare of `Value` with `IgnoreCase | IgnoreNonSpace`, as today.

  No `Convert`, no try/catch. `Compare(MaviTableRow?, MaviTableRow?)` delegates via `GetCell`.
- `TableDataCollection.FilterAndSort` sorts with `OrderBy(r => r.GetCell(SortColumnKey), sorter)` / `OrderByDescending`, so the key is computed once per row; column filters use `GetCell`.
- `MaviTableCell.Guid` is created on first read: `get => field ??= System.Guid.NewGuid().ToString(); set => field = value;`.
- The factory computes `MaviTableColumn.InferColumnType` once per column, not per cell.
- Both tables:
  - Per-row loops use `VisibleColumns` and `row.GetCell(column.Key)`.
  - In the DropDown display style, a row's context-menu items render only `@if (row.ContextMenuVisible)`.

- [ ] **Step 1: Write tests:**
  - `TableSortingTests`:
    - `LongColumn_SortsByValue`: 3000000000, 1, 2000000000 → 1, 2000000000, 3000000000.
    - `DateColumn_SortsChronologically`
    - `EnumColumn_SortsByDeclarationOrder`
    - `TextColumn_SortsIgnoringCase`: "b", "A", "c" → "A", "b", "c".
    - `MissingValues_SortLast`: nullable int 2, null, 1 → 1, 2, null.
    - `Descending_ReversesOrder`
  - `MaviTableCellTests.Guid_IsStableAcrossReads`
  - In `MaviInMemoryTableTests`, `RowContextMenu_RendersItemsOnlyWhenOpen`:
    - a row with one context action titled "Delete", DropDown style
    - before the toggle, no element with text `Delete`
    - after clicking the row's toggle button (`[data-context-menu-id='<row.ContextMenuId>'] button`), one.
- [ ] **Step 2: Run** `--filter-class "*TableSortingTests"` and `--filter-class "*MaviInMemoryTableTests"`. Expected: `LongColumn_SortsByValue` and `RowContextMenu_RendersItemsOnlyWhenOpen` fail; the rest pass.
- [ ] **Step 3: Implement** the interfaces above.
- [ ] **Step 4: Run the full suite** and build the solution. Expected: all pass, 0 warnings from changed files.
- [ ] **Step 5: Commit** `perf: index table cells, compare sort keys by type, render row menus on demand`.
