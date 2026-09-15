# WysiwygEditor — Design Spec

**Date:** 2026-09-14
**Component:** `Maviray.Blazor.Components.Material` → `Components/Editors/Wysiwyg/WysiwygEditor`
**Status:** Approved design, pending implementation plan

## 1. Overview

A lightweight rich-text (WYSIWYG) editor for the Maviray Material component library, modelled on Summernote's minimal editor in look and behaviour but **without** its jQuery/Bootstrap dependencies. Editing is done on a native `contenteditable` surface driven by a thin ES-module JS interop layer, consistent with the library's existing interop components (`MaviInputDate`, `MaviMultiSelect`, `MaviDropdown`).

### Goals
- Reproduce the toolbar and editing behaviour shown in the reference screenshot (minus video and help).
- Emit/consume content as an **HTML string**.
- Give the host explicit, on-demand control over content flow: load-from-provider (Refresh), push-out (Save), and programmatic get/set.
- Fit the library's design system: Tailwind 4 theme tokens, Lineicons/inline-SVG icons, `MaviComponentBase` conventions.

### Non-goals (v1)
- Video embedding.
- Help dialog.
- Collaborative/real-time editing.
- Configurable/removable toolbar groups (fixed toolbar for v1; noted as future work).
- Markdown output (HTML only).

## 2. Reference → toolbar mapping

Left → right, exactly as in the screenshot except the two removed items:

| # | Screenshot item | Implemented as |
|---|-----------------|----------------|
| 1 | Magic-wand dropdown | **Style** dropdown: Normal (`p`), Heading 1–6 (`h1`–`h6`), Quote (`blockquote`), Code block (`pre`) |
| 2 | B / U / eraser | **Font style** group: Bold, Italic, Underline, Strikethrough, Clear formatting *(Italic + Strikethrough added per decision)* |
| 3 | Helvetica ▾ | **Font family** dropdown (configurable list) |
| 4 | A ▾ | **Font color** dropdown: text-color palette + highlight (background) color |
| 5 | Bulleted / numbered | **Lists**: Unordered, Ordered |
| 6 | Lines ▾ | **Paragraph** dropdown: align left/center/right/justify, indent, outdent |
| 7 | Grid ▾ | **Table** dropdown: hover-grid picker → insert N×M table |
| 8 | Link / Image | **Link** (dialog: text + URL), **Image** (file-pick → base64 data URL embedded in content) |
| 9 | Fullscreen / `</>` | **Fullscreen** toggle, **Code view** toggle (surface ↔ raw-HTML textarea) |
| — | Video, Help | **Removed** |

## 3. Architecture

### 3.1 Files
| File | Purpose |
|------|---------|
| `Components/Editors/Wysiwyg/WysiwygEditor.razor` | Markup: toolbar, editing surface, code-view textarea, hidden `InputFile`, link dialog |
| `Components/Editors/Wysiwyg/WysiwygEditor.razor.cs` | Code-behind: parameters, public API, toolbar handlers, JS interop lifecycle |
| `wwwroot/js/maviWysiwygJsInterop.js` | ES module: exec commands, get/set HTML, selection save/restore, image/table insert, fullscreen, dispose |

Code-behind is used (rather than inline `@code`) because this component is substantially larger than the library's inline-`@code` components; the library already keeps large logic in dedicated `.cs` files (e.g. `MaviInputDateBase.cs`, `MenuBase.cs`).

### 3.2 Base class & interfaces
- Inherits **`MaviComponentBase`** (provides `Id`, `Class`, `Style`, `Title`, `Logger`, `EnableLifeCycleLogging`, `HasRendered`, `ComponentOptions`).
- Implements **`IAsyncDisposable`** (to dispose the JS module + `DotNetObjectReference`), following `MaviInputDateBase`.

> The editor is intentionally **not** a `MaviMaterialInputBase<string>` subclass: its content-flow model (provider/save/programmatic, no floating label or per-keystroke validation) does not fit the single-line input base. Form integration is offered via `@bind-Content` instead.

### 3.3 JS interop lifecycle (mirrors `MaviInputDateBase`)
```csharp
// OnAfterRenderAsync(firstRender):
_jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
    "import", "./_content/Maviray.Blazor.Components.Material/js/maviWysiwygJsInterop.js");
_dotNetRef = DotNetObjectReference.Create(this);
await _jsModule.InvokeVoidAsync("initialize", _editorRef, _dotNetRef);
// then load initial Content into the surface.

// DisposeAsync():
if (_jsModule != null) {
    await _jsModule.InvokeVoidAsync("dispose", _editorRef);
    await _jsModule.DisposeAsync();
}
_dotNetRef?.Dispose();
```
Interactive render mode (Server or WASM) is required by the host, as with every JS-interop component in the library. All interop is guarded to `OnAfterRenderAsync`/post-first-render so prerender/SSR does not fault.

## 4. Editing engine (JS module)

Formatting uses `document.execCommand` — deprecated but universally supported and the exact mechanism this class of editor relies on.

### 4.1 Exported functions
| Function | Behaviour |
|----------|-----------|
| `initialize(editorEl, dotNetRef)` | Attach `input` + `blur` listeners; on blur, `dotNetRef.invokeMethodAsync('OnEditorBlurAsync', html)` |
| `exec(cmd, value)` | Restore saved selection, focus surface, `document.execCommand(cmd, false, value)` |
| `formatBlock(tag)` | `exec('formatBlock', '<tag>')` |
| `setFontName(name)` / `setForeColor(c)` / `setBackColor(c)` | `exec('fontName'/'foreColor'/'hiliteColor', …)` |
| `insertLink(url, text)` | Insert `<a href target="_blank">` at caret (or wrap selection) |
| `insertImage(dataUrl)` | `exec('insertImage', dataUrl)` at saved caret |
| `insertTable(rows, cols)` | Build bordered `<table>` HTML, insert at caret |
| `getHtml(editorEl)` / `setHtml(editorEl, html)` | Read/write `innerHTML` |
| `getText(editorEl)` | Read `innerText` (plain text) |
| `saveSelection()` / `restoreSelection()` | Persist the current Range so toolbar dropdowns/dialogs don't lose the caret |
| `focus(editorEl)` | Focus the surface |
| `toggleFullscreen(containerEl, on)` | Toggle body scroll-lock helper (visual fullscreen is CSS-driven from C#) |
| `dispose(editorEl)` | Remove listeners |

### 4.2 Selection handling
The surface's selection is saved on `blur`/before any toolbar interaction and restored before running a command, so opening a dropdown, colour palette, table grid, or link dialog never collapses the user's selection.

### 4.3 Image insertion (base64)
1. Image button triggers a hidden Blazor `InputFile`.
2. C# reads the picked file (`IBrowserFile`) into bytes → builds a `data:{contentType};base64,{...}` URL.
3. C# calls `insertImage(dataUrl)`; the data URL is embedded directly in the content (no server/URL round-trip), matching the decision "pick file and save as base64 string as part of the content."
4. A `MaxImageBytes` guard rejects oversized files (default e.g. 5 MB) to protect Blazor Server circuits.

### 4.4 Code view
Handled in C# for testability: toggling reads `getHtml()` into a bound `<textarea>`; toggling back calls `setHtml(textarea)`. While in code view the rich surface is hidden and the textarea shows/edits raw HTML.

### 4.5 Fullscreen
C# toggles a bool → the outer container gains `fixed inset-0 z-50 …` classes to fill the viewport; `toggleFullscreen` locks body scroll. Toggling again restores inline layout.

## 5. Public API

### 5.1 Parameters
| Parameter | Type | Default | Purpose |
|-----------|------|---------|---------|
| `Content` | `string?` | `null` | Initial/bound HTML. Loaded into the surface on first render. |
| `ContentChanged` | `EventCallback<string?>` | — | Enables `@bind-Content`; raised on **blur** (see §6). |
| `Placeholder` | `string?` | `null` | Shown via CSS when the surface is empty. |
| `ReadOnly` | `bool` | `false` | Surface not editable; toolbar hidden/disabled. |
| `Disabled` | `bool` | `false` | Fully disabled + dimmed. |
| `MinHeight` | `string` | `"240px"` | Min height of the editing surface (applied via style). |
| `FontFamilies` | `IEnumerable<string>?` | standard list | Font-family dropdown options. |
| `ColorPalette` | `IEnumerable<string>?` | Summernote-like palette | Colours offered by the colour dropdown. |
| `ShowRefreshButton` | `bool` | `false` | Show the Refresh action. |
| `RefreshContentProvider` | `Func<Task<string?>>?` | `null` | Invoked by Refresh; returned HTML is loaded. **Null → surface cleared.** |
| `ShowSaveButton` | `bool` | `false` | Show the Save action. |
| `OnSave` | `EventCallback<string>` | — | Invoked with current HTML on Save. |
| `ShowCodeViewButton` | `bool` | `true` | Show the `</>` toggle. |
| `ShowFullscreenButton` | `bool` | `true` | Show the fullscreen toggle. |
| plus `Id`, `Class`, `Style`, `Title` | | | From `MaviComponentBase`. |

### 5.2 Public methods (via `@ref`)
| Method | Purpose |
|--------|---------|
| `Task<string> GetContentAsync()` | Grab current HTML from the surface (works at init and any time later). |
| `Task SetContentAsync(string? html)` | Push HTML into the surface. |
| `Task RefreshAsync()` | Programmatic equivalent of the Refresh button (invokes provider or clears). |
| `Task ClearAsync()` | Empty the surface. |
| `Task FocusAsync()` | Focus the editing surface. |
| `Task<string> GetTextAsync()` | Plain-text content (`innerText`). |

### 5.3 JSInvokable callback
- `OnEditorBlurAsync(string html)` — updates internal content and raises `ContentChanged`.

## 6. Content-sync semantics

Content is grabbed **on demand** — the model is not updated on every keystroke.

- **Refresh** (`ShowRefreshButton` + `RefreshContentProvider`): pulls HTML **into** the editor from the external provider callback; a null provider clears the editor.
- **Save** (`ShowSaveButton` + `OnSave`): pushes the current HTML **out** to the host for persistence.
- **Programmatic**: `GetContentAsync` / `SetContentAsync` for full host control.
- **`ContentChanged`**: raised on **blur** so `@bind-Content` stays sensible without per-keystroke churn. (If the host prefers zero auto-sync, leaving `Content` unbound and using Save/`GetContentAsync` achieves that; a future `AutoSyncOnBlur` flag can make blur-sync opt-out if needed.)

## 7. Styling & icons

- **Tailwind theme tokens only** (`bg-(--theme-…)`, `text-(--theme-…)`, `border-(--theme-…)`, plus `Tailwind.Theme.*` constants and `CssManager.Combine`) — no raw hex, matching `material-css-styling`.
- Visual target: light Summernote look — grouped, bordered toolbar that **wraps** on narrow widths; white editing surface; bottom resize affordance (`resize-y`).
- **Icons:** Lineicons (`lni lni-*`), the library's icon font, wherever a matching glyph exists. For any toolbar glyph the free Lineicons set lacks (e.g. strikethrough, indent/outdent, align variants), fall back to **inline SVG** — the same fallback the library already uses (e.g. `MaviDatePicker` chevrons). *Plan step: verify each required glyph and record the final icon/SVG per button.*
- **Toolbar buttons:** reuse `MaviButtonIcon` where it fits (icon, sizes, theme variants); otherwise small local buttons styled with the same tokens.
- **Dropdowns/popovers** (style, font, colour, paragraph, table): custom lightweight popovers so each item can carry an icon and run a command directly, reusing the shared outside-click listener (`registerGlobalOutsideClickListener` in `maviJsInterop.js` + `data-context-menu-id`). `MaviDropDownMenu` was considered but its text-only `MenuItem` list doesn't fit the icon/grid/palette content.
- **Link dialog:** `MaviDialog` + `MaviInputString`.

## 8. Accessibility
- Toolbar buttons are real `<button type="button">` with `title`/`aria-label`.
- Editing surface: `role="textbox"`, `aria-multiline="true"`, `aria-label` from `Title`.
- `aria-pressed` reflects active state where feasible (bold/italic/etc.).
- Focus never trapped except intentionally within fullscreen.

## 9. Testing (bUnit)

Tests live in `tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg/`, using `ComponentTestBase` (registers `AddMaviComponents` + logging), xUnit `[Fact]`, FluentAssertions, and **loose JS-interop mode** (`JSInterop.Mode = Loose`) so the module import + `InvokeVoidAsync` calls no-op cleanly.

Coverage:
1. Renders toolbar; **omits video and help** buttons.
2. `ShowRefreshButton` / `ShowSaveButton` gate their buttons.
3. `Save` click invokes `OnSave` (grabs content first).
4. `Refresh` with a provider loads returned HTML; with a null provider clears.
5. `SetContentAsync` / `GetContentAsync` call the right module functions (verified via the bUnit JS-interop invocation handlers).
6. Code-view toggle swaps surface ↔ textarea.
7. `ReadOnly`/`Disabled` hide/disable the toolbar and make the surface non-editable.
8. Style/font/colour/paragraph/table popovers open and issue the expected commands.

## 10. Assumptions & open items
- `document.execCommand` is acceptable (standard for this editor class; revisit only if a target browser drops it).
- Base64 images are embedded inline; large-image guard via `MaxImageBytes`.
- Fixed toolbar for v1; configurable toolbar groups are future work.
- Exact Lineicons-vs-SVG choice per button is finalised during implementation (§7).

## 11. File manifest
- **New:** `Components/Editors/Wysiwyg/WysiwygEditor.razor`
- **New:** `Components/Editors/Wysiwyg/WysiwygEditor.razor.cs`
- **New:** `wwwroot/js/maviWysiwygJsInterop.js`
- **New:** `tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg/WysiwygEditorTests.cs`
- **New:** demo page `samples/Maviray.Blazor.Components.Samples.Material.Client/Pages/Editors/PageWysiwygEditor.razor` (new "Editors" category), following the `Page<Component>.razor` convention, wiring up `@bind-Content`, `RefreshContentProvider`, and `OnSave`. Register it in the samples nav menu.
