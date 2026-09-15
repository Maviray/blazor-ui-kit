# WysiwygEditor Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a Summernote-style WYSIWYG editor component (`WysiwygEditor`) for the Maviray Material library using a native `contenteditable` surface plus a thin JS interop module — no jQuery/Bootstrap.

**Architecture:** A `MaviComponentBase`-derived Blazor component renders a grouped toolbar and a `contenteditable` editing surface. Formatting runs through `document.execCommand` in an ES module (`maviWysiwygJsInterop.js`) loaded via `IJSObjectReference` exactly like `MaviInputDateBase`. Content flows on demand: `@bind-Content` (blur-synced), a `RefreshContentProvider` callback (load-in), an `OnSave` callback (push-out), and programmatic `GetContentAsync`/`SetContentAsync`. Images are picked via a hidden Blazor `InputFile`, read to a base64 data URL in C#, and embedded inline.

**Tech Stack:** .NET 10 / Blazor, Tailwind 4 theme tokens (`bg-(--theme-…)`), inline SVG icons, bUnit 2.10.3 + xunit.v3 + AwesomeAssertions.

**Reference spec:** `docs/superpowers/specs/2026-09-14-wysiwyg-editor-design.md`

---

## Conventions used throughout

- **Namespace (component):** `Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg`
- **JS module path:** `./_content/Maviray.Blazor.Components.Material/js/maviWysiwygJsInterop.js`
- **Test namespace:** `Maviray.Blazor.Components.Material.Tests.Components.Editors.Wysiwyg`
- **Test base:** `ComponentTestBase` (already registers `AddMaviComponents()` + logging). Render with `Render<WysiwygEditor>(...)`. Assertions use AwesomeAssertions (`.Should()`).
- **JS in tests:** set `JSInterop.Mode = JSRuntimeMode.Loose;` in each test (or a local base) so the module `import` and every `InvokeVoidAsync`/`InvokeAsync` no-op. For calls whose return value matters, register `JSInterop.Setup<string>("getHtml", _ => true).SetResult("<p>x</p>")` style handlers.
- **Build/test commands** (run from repo root `C:\Repos\GitHub\blazor-ui-kit`):
  - Build library: `dotnet build src/Maviray.Blazor.Components.Material/Maviray.Blazor.Components.Material.csproj`
  - Run tests: `dotnet test tests/Maviray.Blazor.Components.Material.Tests/Maviray.Blazor.Components.Material.Tests.csproj`
  - Run one test: append `--filter "FullyQualifiedName~WysiwygEditorTests.MethodName"`
- **Commit style:** `feat: …` / `test: …` / `docs: …`. End every commit message body with:
  `Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>`

---

## File structure (locked)

| File | Responsibility |
|------|----------------|
| `src/.../Components/Editors/Wysiwyg/WysiwygEditor.razor` | Markup: toolbar, surface, code-view textarea, hidden `InputFile`, link dialog |
| `src/.../Components/Editors/Wysiwyg/WysiwygEditor.razor.cs` | Parameters, public API, toolbar handlers, JS interop lifecycle |
| `src/.../Components/Editors/Wysiwyg/WysiwygIcons.cs` | `static` inline-SVG icon strings (as `MarkupString` sources) |
| `src/.../Components/Editors/Wysiwyg/WysiwygToolbarModels.cs` | Small record/enum types (block styles, alignment, color palette default) |
| `src/.../wwwroot/js/maviWysiwygJsInterop.js` | ES module: exec, get/set HTML, selection, insert image/table/link, fullscreen, dispose |
| `tests/.../Components/Editors/Wysiwyg/WysiwygEditorTests.cs` | bUnit tests |
| `samples/…Client/Pages/Editors/PageWysiwygEditor.razor` | Demo page |
| `samples/…Client/Services/SampleCircuitStateService.cs` | Add "Editors" nav group (modify) |

(`src/...` = `src/Maviray.Blazor.Components.Material`; `tests/...` = `tests/Maviray.Blazor.Components.Material.Tests`.)

---

## Task 1: Scaffold component (renders toolbar shell + surface, JS init)

**Files:**
- Modify: `src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg/WysiwygEditor.razor` (currently empty)
- Create: `src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg/WysiwygEditor.razor.cs`
- Test: `tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg/WysiwygEditorTests.cs`

- [ ] **Step 1: Write the failing test**

Create the test file:

```csharp
using Bunit;
using Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

namespace Maviray.Blazor.Components.Material.Tests.Components.Editors.Wysiwyg;

public class WysiwygEditorTests : ComponentTestBase
{
    public WysiwygEditorTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Renders_Toolbar_And_EditableSurface()
    {
        var cut = Render<WysiwygEditor>();

        cut.FindAll("[data-mavi-wysiwyg-toolbar]").Count.Should().Be(1);

        var surface = cut.Find("[data-mavi-wysiwyg-surface]");
        surface.GetAttribute("contenteditable").Should().Be("true");
        surface.GetAttribute("role").Should().Be("textbox");
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/Maviray.Blazor.Components.Material.Tests/Maviray.Blazor.Components.Material.Tests.csproj --filter "FullyQualifiedName~WysiwygEditorTests"`
Expected: FAIL — `WysiwygEditor` type not found / does not compile.

- [ ] **Step 3: Write the code-behind skeleton**

Create `WysiwygEditor.razor.cs`:

```csharp
using Maviray.Blazor.Components.Core.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

public partial class WysiwygEditor : MaviComponentBase, IAsyncDisposable
{
    private const string JsModulePath =
        "./_content/Maviray.Blazor.Components.Material/js/maviWysiwygJsInterop.js";

    private ElementReference _surfaceRef;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<WysiwygEditor>? _dotNetRef;
    private bool _initialized;

    [Inject] private IJSRuntime? JsRuntime { get; set; }

    #region Content parameters

    [Parameter] public string? Content { get; set; }
    [Parameter] public EventCallback<string?> ContentChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string MinHeight { get; set; } = "240px";

    #endregion

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender || JsRuntime is null)
        {
            return;
        }

        _jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", JsModulePath);
        _dotNetRef = DotNetObjectReference.Create(this);
        await _jsModule.InvokeVoidAsync("initialize", _surfaceRef, _dotNetRef);

        if (!string.IsNullOrEmpty(Content))
        {
            await _jsModule.InvokeVoidAsync("setHtml", _surfaceRef, Content);
        }

        _initialized = true;
    }

    private bool IsInteractive => !ReadOnly && !Disabled;

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            try
            {
                await _jsModule.InvokeVoidAsync("dispose", _surfaceRef);
                await _jsModule.DisposeAsync();
            }
            catch (JSDisconnectedException) { /* circuit gone */ }
        }

        _dotNetRef?.Dispose();
    }
}
```

- [ ] **Step 4: Write the markup skeleton**

Replace `WysiwygEditor.razor` contents:

```razor
@namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg
@using Microsoft.AspNetCore.Components.Forms

<div id="@Id" class="@GetRootCss()" style="@Style" data-mavi-wysiwyg-root>

    <div class="flex flex-wrap items-center gap-1 border-b border-(--theme-light-six) bg-(--theme-light-two) px-2 py-1.5"
         data-mavi-wysiwyg-toolbar
         role="toolbar"
         aria-label="Editor toolbar"
         hidden="@ReadOnly">
        @* Toolbar groups added in later tasks *@
    </div>

    <div @ref="_surfaceRef"
         class="w-full overflow-auto bg-white px-4 py-3 text-(--theme-dark-nine) outline-none resize-y focus:outline-none"
         style="min-height:@MinHeight"
         contenteditable="@IsInteractive.ToString().ToLowerInvariant()"
         role="textbox"
         aria-multiline="true"
         aria-label="@Title"
         data-placeholder="@Placeholder"
         data-mavi-wysiwyg-surface></div>

</div>

@code {
    private string GetRootCss()
    {
        var css = "rounded-md border border-(--theme-light-six) bg-white";
        if (Disabled)
        {
            css += " opacity-60 pointer-events-none";
        }
        return string.IsNullOrEmpty(Class) ? css : $"{css} {Class}";
    }
}
```

- [ ] **Step 5: Create a minimal JS module so `import` resolves at runtime**

Create `src/Maviray.Blazor.Components.Material/wwwroot/js/maviWysiwygJsInterop.js`:

```javascript
export function initialize(surface, dotNetRef) {
    if (!surface) return;
    surface._dotNetRef = dotNetRef;
}

export function setHtml(surface, html) {
    if (surface) surface.innerHTML = html ?? '';
}

export function getHtml(surface) {
    return surface ? surface.innerHTML : '';
}

export function dispose(surface) {
    if (surface) delete surface._dotNetRef;
}
```

- [ ] **Step 6: Run test to verify it passes**

Run: `dotnet test tests/Maviray.Blazor.Components.Material.Tests/Maviray.Blazor.Components.Material.Tests.csproj --filter "FullyQualifiedName~WysiwygEditorTests"`
Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg src/Maviray.Blazor.Components.Material/wwwroot/js/maviWysiwygJsInterop.js
git commit -m "feat: scaffold WysiwygEditor component shell with JS init"
```

---

## Task 2: Complete the JS interop engine

Replace the stub module with the full engine now, so later C# tasks only call already-existing functions. The engine keeps a saved `Range` so toolbar interactions never lose the caret.

**Files:**
- Modify: `src/Maviray.Blazor.Components.Material/wwwroot/js/maviWysiwygJsInterop.js`
- Test: `tests/.../Components/Editors/Wysiwyg/WysiwygEditorTests.cs`

This task has no bUnit test of its own: the ES module cannot be unit-tested in the C#/bUnit setup, and its functions are exercised by the C# tests in Tasks 3–11. Verification here is a successful build (the module ships as a static web asset) plus a manual read-through against the function signatures those tasks call.

- [ ] **Step 1: Implement the full module**

Replace `maviWysiwygJsInterop.js` with:

```javascript
// Per-surface saved selection range, keyed via a WeakMap.
const _savedRanges = new WeakMap();

function saveSelection(surface) {
    const sel = window.getSelection();
    if (sel && sel.rangeCount > 0) {
        const range = sel.getRangeAt(0);
        if (surface.contains(range.commonAncestorContainer)) {
            _savedRanges.set(surface, range.cloneRange());
        }
    }
}

function restoreSelection(surface) {
    const range = _savedRanges.get(surface);
    const sel = window.getSelection();
    surface.focus();
    if (range && sel) {
        sel.removeAllRanges();
        sel.addRange(range);
    }
}

export function initialize(surface, dotNetRef) {
    if (!surface) return;
    surface._dotNetRef = dotNetRef;

    surface._onBlur = () => {
        saveSelection(surface);
        dotNetRef?.invokeMethodAsync('OnEditorBlurAsync', surface.innerHTML);
    };
    surface._onKeyUp = () => saveSelection(surface);
    surface._onMouseUp = () => saveSelection(surface);

    surface.addEventListener('blur', surface._onBlur);
    surface.addEventListener('keyup', surface._onKeyUp);
    surface.addEventListener('mouseup', surface._onMouseUp);
}

export function exec(surface, command, value) {
    restoreSelection(surface);
    document.execCommand(command, false, value ?? null);
    saveSelection(surface);
    return surface.innerHTML;
}

export function formatBlock(surface, tag) {
    return exec(surface, 'formatBlock', '<' + tag + '>');
}

export function setFontName(surface, name) {
    return exec(surface, 'fontName', name);
}

export function setForeColor(surface, color) {
    return exec(surface, 'foreColor', color);
}

export function setBackColor(surface, color) {
    // styleWithCSS makes hiliteColor produce inline style spans reliably.
    document.execCommand('styleWithCSS', false, true);
    const html = exec(surface, 'hiliteColor', color);
    document.execCommand('styleWithCSS', false, false);
    return html;
}

export function insertLink(surface, url, text) {
    restoreSelection(surface);
    const sel = window.getSelection();
    if (sel && !sel.isCollapsed) {
        document.execCommand('createLink', false, url);
        // Force target=_blank on the just-created anchor(s).
        const anchor = sel.anchorNode?.parentElement?.closest('a');
        if (anchor) { anchor.target = '_blank'; anchor.rel = 'noopener'; }
    } else {
        const safeText = (text && text.length) ? text : url;
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.target = '_blank';
        anchor.rel = 'noopener';
        anchor.textContent = safeText;
        const range = _savedRanges.get(surface);
        if (range) { range.insertNode(anchor); }
        else { surface.appendChild(anchor); }
    }
    saveSelection(surface);
    return surface.innerHTML;
}

export function insertImage(surface, dataUrl) {
    return exec(surface, 'insertImage', dataUrl);
}

export function insertTable(surface, rows, cols) {
    let html = '<table style="border-collapse:collapse;width:100%">';
    for (let r = 0; r < rows; r++) {
        html += '<tr>';
        for (let c = 0; c < cols; c++) {
            html += '<td style="border:1px solid #ccc;padding:6px;min-width:32px">&#8203;</td>';
        }
        html += '</tr>';
    }
    html += '</table><p>&#8203;</p>';
    return exec(surface, 'insertHTML', html);
}

export function getHtml(surface) {
    return surface ? surface.innerHTML : '';
}

export function getText(surface) {
    return surface ? surface.innerText : '';
}

export function setHtml(surface, html) {
    if (surface) surface.innerHTML = html ?? '';
}

export function focus(surface) {
    if (surface) surface.focus();
}

export function toggleFullscreen(on) {
    document.body.style.overflow = on ? 'hidden' : '';
}

export function dispose(surface) {
    if (!surface) return;
    surface.removeEventListener('blur', surface._onBlur);
    surface.removeEventListener('keyup', surface._onKeyUp);
    surface.removeEventListener('mouseup', surface._onMouseUp);
    _savedRanges.delete(surface);
    delete surface._dotNetRef;
}
```

- [ ] **Step 2: Build to confirm no syntax errors ship**

Run: `dotnet build src/Maviray.Blazor.Components.Material/Maviray.Blazor.Components.Material.csproj`
Expected: Build succeeded (JS is copied as a static web asset).

- [ ] **Step 3: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/wwwroot/js/maviWysiwygJsInterop.js
git commit -m "feat: implement WysiwygEditor JS interop engine"
```

---

## Task 3: Content API + sync (get/set/clear/focus, blur callback, Refresh, Save)

**Files:**
- Modify: `src/.../Components/Editors/Wysiwyg/WysiwygEditor.razor.cs`
- Modify: `src/.../Components/Editors/Wysiwyg/WysiwygEditor.razor` (toolbar Save/Refresh buttons)
- Test: `tests/.../WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing tests**

Add to `WysiwygEditorTests.cs`:

```csharp
[Fact]
public async Task GetContentAsync_ReturnsHtml_FromModule()
{
    JSInterop.Setup<string>("getHtml", _ => true).SetResult("<p>hello</p>");

    var cut = Render<WysiwygEditor>();
    var html = await cut.Instance.GetContentAsync();

    html.Should().Be("<p>hello</p>");
}

[Fact]
public async Task SetContentAsync_CallsModule_And_RaisesContentChanged()
{
    JSInterop.SetupVoid("setHtml", _ => true);
    string? changed = null;

    var cut = Render<WysiwygEditor>(p => p
        .Add(x => x.ContentChanged, EventCallback.Factory.Create<string?>(this, v => changed = v)));

    await cut.Instance.SetContentAsync("<p>abc</p>");

    changed.Should().Be("<p>abc</p>");
    JSInterop.VerifyInvoke("setHtml");
}

[Fact]
public async Task Save_Invokes_OnSave_WithCurrentHtml()
{
    JSInterop.Setup<string>("getHtml", _ => true).SetResult("<p>saved</p>");
    string? saved = null;

    var cut = Render<WysiwygEditor>(p => p
        .Add(x => x.ShowSaveButton, true)
        .Add(x => x.OnSave, EventCallback.Factory.Create<string>(this, v => saved = v)));

    await cut.Find("[data-mavi-wysiwyg-save]").ClickAsync(new());

    saved.Should().Be("<p>saved</p>");
}

[Fact]
public async Task Refresh_WithProvider_LoadsReturnedHtml()
{
    JSInterop.SetupVoid("setHtml", _ => true);

    var cut = Render<WysiwygEditor>(p => p
        .Add(x => x.ShowRefreshButton, true)
        .Add(x => x.RefreshContentProvider, () => Task.FromResult<string?>("<p>fresh</p>")));

    await cut.Find("[data-mavi-wysiwyg-refresh]").ClickAsync(new());

    JSInterop.VerifyInvoke("setHtml");
    var invocation = JSInterop.Invocations["setHtml"].Last();
    invocation.Arguments[1].Should().Be("<p>fresh</p>");
}

[Fact]
public async Task Refresh_WithNullProvider_ClearsSurface()
{
    JSInterop.SetupVoid("setHtml", _ => true);

    var cut = Render<WysiwygEditor>(p => p.Add(x => x.ShowRefreshButton, true));

    await cut.Find("[data-mavi-wysiwyg-refresh]").ClickAsync(new());

    var invocation = JSInterop.Invocations["setHtml"].Last();
    invocation.Arguments[1].Should().Be(string.Empty);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test … --filter "FullyQualifiedName~WysiwygEditorTests"`
Expected: FAIL — members `ShowSaveButton`, `OnSave`, `ShowRefreshButton`, `RefreshContentProvider`, `GetContentAsync`, `SetContentAsync`, and the save/refresh buttons do not exist.

- [ ] **Step 3: Add the content API to the code-behind**

Add to `WysiwygEditor.razor.cs` (inside the class):

```csharp
#region Save / Refresh parameters

[Parameter] public bool ShowSaveButton { get; set; }
[Parameter] public EventCallback<string> OnSave { get; set; }
[Parameter] public bool ShowRefreshButton { get; set; }
[Parameter] public Func<Task<string?>>? RefreshContentProvider { get; set; }

#endregion

#region Public API

public async Task<string> GetContentAsync()
{
    if (_jsModule is null) return Content ?? string.Empty;
    return await _jsModule.InvokeAsync<string>("getHtml", _surfaceRef);
}

public async Task<string> GetTextAsync()
{
    if (_jsModule is null) return string.Empty;
    return await _jsModule.InvokeAsync<string>("getText", _surfaceRef);
}

public async Task SetContentAsync(string? html)
{
    if (_jsModule is not null)
    {
        await _jsModule.InvokeVoidAsync("setHtml", _surfaceRef, html ?? string.Empty);
    }
    await UpdateContentAsync(html ?? string.Empty);
}

public Task ClearAsync() => SetContentAsync(string.Empty);

public async Task FocusAsync()
{
    if (_jsModule is not null)
    {
        await _jsModule.InvokeVoidAsync("focus", _surfaceRef);
    }
}

public async Task RefreshAsync()
{
    var html = RefreshContentProvider is null
        ? string.Empty
        : await RefreshContentProvider.Invoke() ?? string.Empty;

    await SetContentAsync(html);
}

#endregion

#region Internal helpers

private async Task UpdateContentAsync(string? html)
{
    Content = html;
    if (ContentChanged.HasDelegate)
    {
        await ContentChanged.InvokeAsync(html);
    }
}

[JSInvokable]
public async Task OnEditorBlurAsync(string html)
{
    await UpdateContentAsync(html);
}

private async Task HandleSaveClickAsync()
{
    var html = await GetContentAsync();
    await UpdateContentAsync(html);
    if (OnSave.HasDelegate)
    {
        await OnSave.InvokeAsync(html);
    }
}

private Task HandleRefreshClickAsync() => RefreshAsync();

#endregion
```

- [ ] **Step 4: Add Save/Refresh buttons to the toolbar markup**

In `WysiwygEditor.razor`, inside the `data-mavi-wysiwyg-toolbar` div, add at the end:

```razor
@if (ShowRefreshButton)
{
    <button type="button"
            class="@ToolbarButtonCss()"
            title="Refresh"
            data-mavi-wysiwyg-refresh
            @onclick="HandleRefreshClickAsync">
        @((MarkupString)WysiwygIcons.Refresh)
    </button>
}
@if (ShowSaveButton)
{
    <button type="button"
            class="@ToolbarButtonCss()"
            title="Save"
            data-mavi-wysiwyg-save
            @onclick="HandleSaveClickAsync">
        @((MarkupString)WysiwygIcons.Save)
    </button>
}
```

Add the shared button-CSS helper to the `@code` block in the `.razor` (used by all toolbar buttons):

```razor
@code {
    private static string ToolbarButtonCss() =>
        "inline-flex h-8 w-8 items-center justify-center rounded text-(--theme-dark-eight) " +
        "hover:bg-(--theme-light-four) active:bg-(--theme-light-five) transition-colors cursor-pointer";
}
```

> Note: the existing `GetRootCss()` `@code` block from Task 1 stays; add `ToolbarButtonCss` alongside it.

- [ ] **Step 5: Create the icon file (with the two icons needed so far)**

Create `src/.../Components/Editors/Wysiwyg/WysiwygIcons.cs`. Icons are 20×20 stroke SVGs using `currentColor`. Full set is filled in across tasks; start with:

```csharp
namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

internal static class WysiwygIcons
{
    private const string Open =
        "<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" " +
        "stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\">";
    private const string Close = "</svg>";

    public static readonly string Refresh = Open +
        "<polyline points=\"23 4 23 10 17 10\"/><polyline points=\"1 20 1 14 7 14\"/>" +
        "<path d=\"M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15\"/>" + Close;

    public static readonly string Save = Open +
        "<path d=\"M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z\"/>" +
        "<polyline points=\"17 21 17 13 7 13 7 21\"/><polyline points=\"7 3 7 8 15 8\"/>" + Close;
}
```

- [ ] **Step 6: Run tests to verify they pass**

Run: `dotnet test … --filter "FullyQualifiedName~WysiwygEditorTests"`
Expected: PASS (all content-API tests green).

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor content API, save and refresh"
```

---

## Task 4: Inline formatting group (bold, italic, underline, strikethrough, clear)

**Files:**
- Modify: `WysiwygEditor.razor` (toolbar group), `WysiwygEditor.razor.cs` (exec helper), `WysiwygIcons.cs`
- Test: `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing test**

```csharp
[Fact]
public async Task BoldButton_Invokes_Exec_Bold()
{
    JSInterop.Setup<string>("exec", _ => true).SetResult("<b>x</b>");

    var cut = Render<WysiwygEditor>();

    await cut.Find("[data-mavi-cmd='bold']").ClickAsync(new());

    var invocation = JSInterop.Invocations["exec"].Last();
    invocation.Arguments[1].Should().Be("bold");
}

[Fact]
public void InlineGroup_Renders_AllFiveButtons()
{
    var cut = Render<WysiwygEditor>();

    cut.FindAll("[data-mavi-cmd='bold']").Count.Should().Be(1);
    cut.FindAll("[data-mavi-cmd='italic']").Count.Should().Be(1);
    cut.FindAll("[data-mavi-cmd='underline']").Count.Should().Be(1);
    cut.FindAll("[data-mavi-cmd='strikeThrough']").Count.Should().Be(1);
    cut.FindAll("[data-mavi-cmd='removeFormat']").Count.Should().Be(1);
}
```

- [ ] **Step 2: Run to verify fail**

Run: `dotnet test … --filter "FullyQualifiedName~WysiwygEditorTests"`
Expected: FAIL — buttons/`ExecAsync` missing.

- [ ] **Step 3: Add `ExecAsync` to the code-behind**

Add to `WysiwygEditor.razor.cs`:

```csharp
private async Task ExecAsync(string command, string? value = null)
{
    if (_jsModule is null || !IsInteractive) return;
    var html = await _jsModule.InvokeAsync<string>("exec", _surfaceRef, command, value);
    await UpdateContentAsync(html);
}
```

- [ ] **Step 4: Add the inline group markup**

In `WysiwygEditor.razor`, add a group at the top of the toolbar (before Save/Refresh):

```razor
<div class="flex items-center gap-0.5 pr-1" role="group" aria-label="Font style">
    <button type="button" class="@ToolbarButtonCss()" title="Bold" data-mavi-cmd="bold"
            @onclick='() => ExecAsync("bold")'>@((MarkupString)WysiwygIcons.Bold)</button>
    <button type="button" class="@ToolbarButtonCss()" title="Italic" data-mavi-cmd="italic"
            @onclick='() => ExecAsync("italic")'>@((MarkupString)WysiwygIcons.Italic)</button>
    <button type="button" class="@ToolbarButtonCss()" title="Underline" data-mavi-cmd="underline"
            @onclick='() => ExecAsync("underline")'>@((MarkupString)WysiwygIcons.Underline)</button>
    <button type="button" class="@ToolbarButtonCss()" title="Strikethrough" data-mavi-cmd="strikeThrough"
            @onclick='() => ExecAsync("strikeThrough")'>@((MarkupString)WysiwygIcons.Strikethrough)</button>
    <button type="button" class="@ToolbarButtonCss()" title="Clear formatting" data-mavi-cmd="removeFormat"
            @onclick='() => ExecAsync("removeFormat")'>@((MarkupString)WysiwygIcons.Eraser)</button>
</div>
```

- [ ] **Step 5: Add the five icons to `WysiwygIcons.cs`**

```csharp
public static readonly string Bold = Open +
    "<path d=\"M6 4h8a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/><path d=\"M6 12h9a4 4 0 0 1 4 4 4 4 0 0 1-4 4H6z\"/>" + Close;

public static readonly string Italic = Open +
    "<line x1=\"19\" y1=\"4\" x2=\"10\" y2=\"4\"/><line x1=\"14\" y1=\"20\" x2=\"5\" y2=\"20\"/>" +
    "<line x1=\"15\" y1=\"4\" x2=\"9\" y2=\"20\"/>" + Close;

public static readonly string Underline = Open +
    "<path d=\"M6 3v7a6 6 0 0 0 6 6 6 6 0 0 0 6-6V3\"/><line x1=\"4\" y1=\"21\" x2=\"20\" y2=\"21\"/>" + Close;

public static readonly string Strikethrough = Open +
    "<line x1=\"4\" y1=\"12\" x2=\"20\" y2=\"12\"/><path d=\"M6 7a4 3 0 0 1 8 0M10 12a4 3 0 0 1 4 3 4 3 0 0 1-8 0\"/>" + Close;

public static readonly string Eraser = Open +
    "<path d=\"M20 20H7L3 16a2 2 0 0 1 0-3l9-9a2 2 0 0 1 3 0l5 5a2 2 0 0 1 0 3l-7 7\"/>" +
    "<line x1=\"18\" y1=\"12.5\" x2=\"9.5\" y2=\"4\"/>" + Close;
```

- [ ] **Step 6: Run tests to verify pass**

Run: `dotnet test … --filter "FullyQualifiedName~WysiwygEditorTests"`
Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor inline formatting group"
```

---

## Task 5: Reusable toolbar popover (open/close via shared outside-click)

The Style, Font, Color, Paragraph, and Table controls are all popovers. Build one reusable open/close mechanism now, reusing the library's global outside-click listener (`registerGlobalOutsideClickListener` in `maviJsInterop.js`) so exactly one popover is open at a time and clicking away closes it.

**Files:**
- Modify: `WysiwygEditor.razor.cs` (popover state + JSInvokable close), `WysiwygEditor.razor` (popover container markup pattern)
- Test: `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing test**

```csharp
[Fact]
public async Task StylePopover_Toggles_Open_And_Closed()
{
    var cut = Render<WysiwygEditor>();

    cut.FindAll("[data-mavi-popover='style']").Count.Should().Be(0);

    await cut.Find("[data-mavi-popover-toggle='style']").ClickAsync(new());
    cut.FindAll("[data-mavi-popover='style']").Count.Should().Be(1);

    await cut.Find("[data-mavi-popover-toggle='style']").ClickAsync(new());
    cut.FindAll("[data-mavi-popover='style']").Count.Should().Be(0);
}
```

- [ ] **Step 2: Run to verify fail**

Expected: FAIL — no style toggle exists yet.

- [ ] **Step 3: Add popover state + outside-click registration to code-behind**

Add to `WysiwygEditor.razor.cs`:

```csharp
private string? _openPopover;

private void TogglePopover(string name)
{
    _openPopover = _openPopover == name ? null : name;
}

private bool IsPopoverOpen(string name) => _openPopover == name;

[JSInvokable]
public void HandleOutsideClick(string clickedId)
{
    if (clickedId != Id)
    {
        _openPopover = null;
        StateHasChanged();
    }
}
```

Register the shared outside-click listener in `OnAfterRenderAsync` (extend Task 1's method, after `_initialized = true;`):

```csharp
await JsRuntime.InvokeVoidAsync(
    "registerGlobalOutsideClickListener", Id, _dotNetRef, nameof(HandleOutsideClick));
```

And unregister in `DisposeAsync` (before disposing the module):

```csharp
try
{
    if (JsRuntime is not null)
    {
        await JsRuntime.InvokeVoidAsync("unregisterGlobalOutsideClickListener", Id);
    }
}
catch (JSDisconnectedException) { }
```

> The root `<div>` already carries `id="@Id"`. Add `data-context-menu-id="@Id"` to it so the shared listener treats clicks inside the editor as "inside".

- [ ] **Step 4: Add the Style popover (first consumer of the pattern)**

In `WysiwygEditor.razor`, add a Style group at the very start of the toolbar. This also introduces `WysiwygBlockStyle` (Task 5 model):

```razor
<div class="relative pr-1">
    <button type="button" class="@ToolbarButtonCss()" title="Style"
            data-mavi-popover-toggle="style" @onclick='() => TogglePopover("style")'>
        @((MarkupString)WysiwygIcons.Paragraph)
    </button>
    @if (IsPopoverOpen("style"))
    {
        <div class="absolute left-0 top-full z-20 mt-1 min-w-40 rounded-md border border-(--theme-light-six) bg-white p-1 shadow-lg"
             data-mavi-popover="style">
            @foreach (var style in WysiwygBlockStyle.All)
            {
                <button type="button"
                        class="block w-full rounded px-3 py-1.5 text-left text-sm hover:bg-(--theme-light-three)"
                        @onclick="() => ApplyBlockStyleAsync(style.Tag)">
                    @style.Label
                </button>
            }
        </div>
    }
</div>
```

- [ ] **Step 5: Add `ApplyBlockStyleAsync` + the block-style model**

Create `src/.../Components/Editors/Wysiwyg/WysiwygToolbarModels.cs`:

```csharp
namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

public sealed record WysiwygBlockStyle(string Label, string Tag)
{
    public static readonly IReadOnlyList<WysiwygBlockStyle> All =
    [
        new("Normal", "p"),
        new("Heading 1", "h1"),
        new("Heading 2", "h2"),
        new("Heading 3", "h3"),
        new("Heading 4", "h4"),
        new("Heading 5", "h5"),
        new("Heading 6", "h6"),
        new("Quote", "blockquote"),
        new("Code", "pre"),
    ];
}
```

Add to `WysiwygEditor.razor.cs`:

```csharp
private async Task ApplyBlockStyleAsync(string tag)
{
    _openPopover = null;
    if (_jsModule is null || !IsInteractive) return;
    var html = await _jsModule.InvokeAsync<string>("formatBlock", _surfaceRef, tag);
    await UpdateContentAsync(html);
}
```

Add the `Paragraph` icon to `WysiwygIcons.cs`:

```csharp
public static readonly string Paragraph = Open +
    "<path d=\"M13 4v16M17 4v16M17 4H9a5 5 0 0 0 0 10h4\"/>" + Close;
```

- [ ] **Step 6: Run tests to verify pass**

Run: `dotnet test … --filter "FullyQualifiedName~WysiwygEditorTests"`
Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor popover pattern and style dropdown"
```

---

## Task 6: Lists + paragraph (alignment, indent/outdent)

**Files:** `WysiwygEditor.razor`, `WysiwygIcons.cs`
**Test:** `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
[Theory]
[InlineData("insertUnorderedList")]
[InlineData("insertOrderedList")]
public async Task ListButtons_Invoke_Exec(string command)
{
    JSInterop.Setup<string>("exec", _ => true).SetResult("<ul></ul>");
    var cut = Render<WysiwygEditor>();

    await cut.Find($"[data-mavi-cmd='{command}']").ClickAsync(new());

    JSInterop.Invocations["exec"].Last().Arguments[1].Should().Be(command);
}

[Fact]
public async Task AlignCenter_Invokes_Exec_justifyCenter()
{
    JSInterop.Setup<string>("exec", _ => true).SetResult("");
    var cut = Render<WysiwygEditor>();

    await cut.Find("[data-mavi-popover-toggle='paragraph']").ClickAsync(new());
    await cut.Find("[data-mavi-cmd='justifyCenter']").ClickAsync(new());

    JSInterop.Invocations["exec"].Last().Arguments[1].Should().Be("justifyCenter");
}
```

- [ ] **Step 2: Run to verify fail** — Expected: FAIL (buttons missing).

- [ ] **Step 3: Add list buttons + paragraph popover to `WysiwygEditor.razor`**

Add after the inline group:

```razor
<div class="flex items-center gap-0.5 pr-1" role="group" aria-label="Lists">
    <button type="button" class="@ToolbarButtonCss()" title="Unordered list" data-mavi-cmd="insertUnorderedList"
            @onclick='() => ExecAsync("insertUnorderedList")'>@((MarkupString)WysiwygIcons.ListUnordered)</button>
    <button type="button" class="@ToolbarButtonCss()" title="Ordered list" data-mavi-cmd="insertOrderedList"
            @onclick='() => ExecAsync("insertOrderedList")'>@((MarkupString)WysiwygIcons.ListOrdered)</button>
</div>

<div class="relative pr-1">
    <button type="button" class="@ToolbarButtonCss()" title="Paragraph"
            data-mavi-popover-toggle="paragraph" @onclick='() => TogglePopover("paragraph")'>
        @((MarkupString)WysiwygIcons.AlignLeft)
    </button>
    @if (IsPopoverOpen("paragraph"))
    {
        <div class="absolute left-0 top-full z-20 mt-1 flex gap-0.5 rounded-md border border-(--theme-light-six) bg-white p-1 shadow-lg"
             data-mavi-popover="paragraph">
            <button type="button" class="@ToolbarButtonCss()" title="Align left" data-mavi-cmd="justifyLeft"
                    @onclick='() => ExecFromPopoverAsync("justifyLeft")'>@((MarkupString)WysiwygIcons.AlignLeft)</button>
            <button type="button" class="@ToolbarButtonCss()" title="Align center" data-mavi-cmd="justifyCenter"
                    @onclick='() => ExecFromPopoverAsync("justifyCenter")'>@((MarkupString)WysiwygIcons.AlignCenter)</button>
            <button type="button" class="@ToolbarButtonCss()" title="Align right" data-mavi-cmd="justifyRight"
                    @onclick='() => ExecFromPopoverAsync("justifyRight")'>@((MarkupString)WysiwygIcons.AlignRight)</button>
            <button type="button" class="@ToolbarButtonCss()" title="Justify" data-mavi-cmd="justifyFull"
                    @onclick='() => ExecFromPopoverAsync("justifyFull")'>@((MarkupString)WysiwygIcons.AlignJustify)</button>
            <button type="button" class="@ToolbarButtonCss()" title="Indent" data-mavi-cmd="indent"
                    @onclick='() => ExecFromPopoverAsync("indent")'>@((MarkupString)WysiwygIcons.Indent)</button>
            <button type="button" class="@ToolbarButtonCss()" title="Outdent" data-mavi-cmd="outdent"
                    @onclick='() => ExecFromPopoverAsync("outdent")'>@((MarkupString)WysiwygIcons.Outdent)</button>
        </div>
    }
</div>
```

- [ ] **Step 4: Add `ExecFromPopoverAsync` (closes popover then execs)**

Add to `WysiwygEditor.razor.cs`:

```csharp
private async Task ExecFromPopoverAsync(string command)
{
    _openPopover = null;
    await ExecAsync(command);
}
```

- [ ] **Step 5: Add icons to `WysiwygIcons.cs`**

```csharp
public static readonly string ListUnordered = Open +
    "<line x1=\"8\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"8\" y1=\"12\" x2=\"21\" y2=\"12\"/>" +
    "<line x1=\"8\" y1=\"18\" x2=\"21\" y2=\"18\"/><line x1=\"3\" y1=\"6\" x2=\"3.01\" y2=\"6\"/>" +
    "<line x1=\"3\" y1=\"12\" x2=\"3.01\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"3.01\" y2=\"18\"/>" + Close;

public static readonly string ListOrdered = Open +
    "<line x1=\"10\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"10\" y1=\"12\" x2=\"21\" y2=\"12\"/>" +
    "<line x1=\"10\" y1=\"18\" x2=\"21\" y2=\"18\"/><path d=\"M4 6h1v4M4 10h2\"/>" +
    "<path d=\"M6 18H4c0-1 2-2 2-3s-1-1.5-2-1\"/>" + Close;

public static readonly string AlignLeft = Open +
    "<line x1=\"17\" y1=\"10\" x2=\"3\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/>" +
    "<line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"17\" y1=\"18\" x2=\"3\" y2=\"18\"/>" + Close;

public static readonly string AlignCenter = Open +
    "<line x1=\"18\" y1=\"10\" x2=\"6\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/>" +
    "<line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"18\" y1=\"18\" x2=\"6\" y2=\"18\"/>" + Close;

public static readonly string AlignRight = Open +
    "<line x1=\"21\" y1=\"10\" x2=\"7\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/>" +
    "<line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"21\" y1=\"18\" x2=\"7\" y2=\"18\"/>" + Close;

public static readonly string AlignJustify = Open +
    "<line x1=\"21\" y1=\"10\" x2=\"3\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/>" +
    "<line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"21\" y1=\"18\" x2=\"3\" y2=\"18\"/>" + Close;

public static readonly string Indent = Open +
    "<polyline points=\"3 8 7 12 3 16\"/><line x1=\"21\" y1=\"6\" x2=\"11\" y2=\"6\"/>" +
    "<line x1=\"21\" y1=\"12\" x2=\"11\" y2=\"12\"/><line x1=\"21\" y1=\"18\" x2=\"11\" y2=\"18\"/>" + Close;

public static readonly string Outdent = Open +
    "<polyline points=\"7 8 3 12 7 16\"/><line x1=\"21\" y1=\"6\" x2=\"11\" y2=\"6\"/>" +
    "<line x1=\"21\" y1=\"12\" x2=\"11\" y2=\"12\"/><line x1=\"21\" y1=\"18\" x2=\"11\" y2=\"18\"/>" + Close;
```

- [ ] **Step 6: Run tests → PASS.** Run the filter command above.

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor lists and paragraph controls"
```

---

## Task 7: Font family + color dropdowns

**Files:** `WysiwygEditor.razor`, `WysiwygEditor.razor.cs`, `WysiwygToolbarModels.cs`, `WysiwygIcons.cs`
**Test:** `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
[Fact]
public async Task FontFamily_Selection_Invokes_setFontName()
{
    JSInterop.Setup<string>("setFontName", _ => true).SetResult("");
    var cut = Render<WysiwygEditor>();

    await cut.Find("[data-mavi-popover-toggle='font']").ClickAsync(new());
    await cut.Find("[data-mavi-font='Georgia']").ClickAsync(new());

    JSInterop.Invocations["setFontName"].Last().Arguments[1].Should().Be("Georgia");
}

[Fact]
public async Task ForeColor_Selection_Invokes_setForeColor()
{
    JSInterop.Setup<string>("setForeColor", _ => true).SetResult("");
    var cut = Render<WysiwygEditor>();

    await cut.Find("[data-mavi-popover-toggle='color']").ClickAsync(new());
    await cut.Find("[data-mavi-forecolor='#e03131']").ClickAsync(new());

    JSInterop.Invocations["setForeColor"].Last().Arguments[1].Should().Be("#e03131");
}
```

- [ ] **Step 2: Run to verify fail** — Expected: FAIL.

- [ ] **Step 3: Add default font + color lists and parameters**

Add to `WysiwygToolbarModels.cs`:

```csharp
public static class WysiwygDefaults
{
    public static readonly IReadOnlyList<string> Fonts =
    [
        "Arial", "Helvetica", "Georgia", "Tahoma", "Times New Roman", "Verdana", "Courier New",
    ];

    public static readonly IReadOnlyList<string> Colors =
    [
        "#000000", "#495057", "#e03131", "#d6336c", "#ae3ec9", "#7048e8", "#1971c2",
        "#0c8599", "#2f9e44", "#f08c00", "#e8590c", "#ffffff",
    ];
}
```

Add parameters to `WysiwygEditor.razor.cs`:

```csharp
[Parameter] public IEnumerable<string>? FontFamilies { get; set; }
[Parameter] public IEnumerable<string>? ColorPalette { get; set; }

private IEnumerable<string> EffectiveFonts => FontFamilies ?? WysiwygDefaults.Fonts;
private IEnumerable<string> EffectiveColors => ColorPalette ?? WysiwygDefaults.Colors;
private string _currentFont = "Helvetica";

private async Task ApplyFontAsync(string font)
{
    _openPopover = null;
    _currentFont = font;
    if (_jsModule is null || !IsInteractive) return;
    var html = await _jsModule.InvokeAsync<string>("setFontName", _surfaceRef, font);
    await UpdateContentAsync(html);
}

private async Task ApplyForeColorAsync(string color)
{
    _openPopover = null;
    if (_jsModule is null || !IsInteractive) return;
    var html = await _jsModule.InvokeAsync<string>("setForeColor", _surfaceRef, color);
    await UpdateContentAsync(html);
}

private async Task ApplyBackColorAsync(string color)
{
    _openPopover = null;
    if (_jsModule is null || !IsInteractive) return;
    var html = await _jsModule.InvokeAsync<string>("setBackColor", _surfaceRef, color);
    await UpdateContentAsync(html);
}
```

- [ ] **Step 4: Add font + color markup to `WysiwygEditor.razor`**

```razor
@* Font family: shows current font name + chevron, matching the screenshot *@
<div class="relative pr-1">
    <button type="button"
            class="inline-flex h-8 items-center gap-1 rounded px-2 text-sm text-(--theme-dark-eight) hover:bg-(--theme-light-four)"
            title="Font family" data-mavi-popover-toggle="font" @onclick='() => TogglePopover("font")'>
        <span>@_currentFont</span>@((MarkupString)WysiwygIcons.ChevronDown)
    </button>
    @if (IsPopoverOpen("font"))
    {
        <div class="absolute left-0 top-full z-20 mt-1 min-w-44 rounded-md border border-(--theme-light-six) bg-white p-1 shadow-lg"
             data-mavi-popover="font">
            @foreach (var font in EffectiveFonts)
            {
                <button type="button"
                        class="block w-full rounded px-3 py-1.5 text-left text-sm hover:bg-(--theme-light-three)"
                        style="font-family:@font" data-mavi-font="@font"
                        @onclick="() => ApplyFontAsync(font)">@font</button>
            }
        </div>
    }
</div>

@* Color: A button opening fore/back palettes *@
<div class="relative pr-1">
    <button type="button" class="@ToolbarButtonCss()" title="Font color"
            data-mavi-popover-toggle="color" @onclick='() => TogglePopover("color")'>
        @((MarkupString)WysiwygIcons.FontColor)
    </button>
    @if (IsPopoverOpen("color"))
    {
        <div class="absolute left-0 top-full z-20 mt-1 w-56 rounded-md border border-(--theme-light-six) bg-white p-2 shadow-lg"
             data-mavi-popover="color">
            <div class="mb-1 text-xs font-medium text-(--theme-dark-six)">Text color</div>
            <div class="mb-2 grid grid-cols-6 gap-1">
                @foreach (var color in EffectiveColors)
                {
                    <button type="button" class="h-6 w-6 rounded border border-(--theme-light-six)"
                            style="background-color:@color" title="@color" data-mavi-forecolor="@color"
                            @onclick="() => ApplyForeColorAsync(color)"></button>
                }
            </div>
            <div class="mb-1 text-xs font-medium text-(--theme-dark-six)">Highlight</div>
            <div class="grid grid-cols-6 gap-1">
                @foreach (var color in EffectiveColors)
                {
                    <button type="button" class="h-6 w-6 rounded border border-(--theme-light-six)"
                            style="background-color:@color" title="@color" data-mavi-backcolor="@color"
                            @onclick="() => ApplyBackColorAsync(color)"></button>
                }
            </div>
        </div>
    }
</div>
```

- [ ] **Step 5: Add icons to `WysiwygIcons.cs`**

```csharp
public static readonly string ChevronDown = Open + "<polyline points=\"6 9 12 15 18 9\"/>" + Close;

// "A" with a colored underline bar — rendered as its own small SVG (uses text).
public static readonly string FontColor =
    "<svg width=\"18\" height=\"18\" viewBox=\"0 0 24 24\" fill=\"none\">" +
    "<text x=\"5\" y=\"16\" font-size=\"15\" font-weight=\"700\" fill=\"currentColor\" " +
    "font-family=\"sans-serif\">A</text><rect x=\"4\" y=\"19\" width=\"14\" height=\"3\" fill=\"#e03131\"/></svg>";
```

- [ ] **Step 6: Run tests → PASS.**

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor font family and color pickers"
```

---

## Task 8: Link dialog

Use `MaviDialog` + `MaviInputString`. If wiring `MaviDialog` proves awkward in a unit test, a lightweight inline popover with two `MaviInputString` fields and Insert/Cancel buttons is an acceptable equivalent — keep the `data-` hooks below either way.

**Files:** `WysiwygEditor.razor`, `WysiwygEditor.razor.cs`, `WysiwygIcons.cs`
**Test:** `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing test**

```csharp
[Fact]
public async Task LinkDialog_Insert_Invokes_insertLink()
{
    JSInterop.Setup<string>("insertLink", _ => true).SetResult("");
    var cut = Render<WysiwygEditor>();

    await cut.Find("[data-mavi-popover-toggle='link']").ClickAsync(new());
    cut.Find("[data-mavi-link-url]").Input("https://example.com");
    cut.Find("[data-mavi-link-text]").Input("Example");
    await cut.Find("[data-mavi-link-insert]").ClickAsync(new());

    var inv = JSInterop.Invocations["insertLink"].Last();
    inv.Arguments[1].Should().Be("https://example.com");
    inv.Arguments[2].Should().Be("Example");
}
```

- [ ] **Step 2: Run to verify fail** — Expected: FAIL.

- [ ] **Step 3: Add link state + handler to code-behind**

```csharp
private string _linkUrl = string.Empty;
private string _linkText = string.Empty;

private async Task InsertLinkAsync()
{
    _openPopover = null;
    if (_jsModule is null || !IsInteractive || string.IsNullOrWhiteSpace(_linkUrl)) return;
    var html = await _jsModule.InvokeAsync<string>("insertLink", _surfaceRef, _linkUrl, _linkText);
    await UpdateContentAsync(html);
    _linkUrl = string.Empty;
    _linkText = string.Empty;
}
```

- [ ] **Step 4: Add link popover markup to `WysiwygEditor.razor`**

```razor
<div class="relative pr-1">
    <button type="button" class="@ToolbarButtonCss()" title="Link"
            data-mavi-popover-toggle="link" @onclick='() => TogglePopover("link")'>
        @((MarkupString)WysiwygIcons.Link)
    </button>
    @if (IsPopoverOpen("link"))
    {
        <div class="absolute left-0 top-full z-20 mt-1 w-64 rounded-md border border-(--theme-light-six) bg-white p-3 shadow-lg"
             data-mavi-popover="link">
            <label class="mb-1 block text-xs font-medium text-(--theme-dark-six)">Text</label>
            <input type="text" value="@_linkText" @oninput="e => _linkText = e.Value?.ToString() ?? string.Empty"
                   data-mavi-link-text
                   class="mb-2 w-full rounded border border-(--theme-light-six) px-2 py-1 text-sm outline-none" />
            <label class="mb-1 block text-xs font-medium text-(--theme-dark-six)">URL</label>
            <input type="text" value="@_linkUrl" @oninput="e => _linkUrl = e.Value?.ToString() ?? string.Empty"
                   data-mavi-link-url
                   class="mb-3 w-full rounded border border-(--theme-light-six) px-2 py-1 text-sm outline-none" />
            <div class="flex justify-end gap-2">
                <button type="button" class="rounded px-3 py-1 text-sm hover:bg-(--theme-light-three)"
                        @onclick='() => _openPopover = null'>Cancel</button>
                <button type="button" class="rounded bg-(--theme-primary-seven) px-3 py-1 text-sm text-white hover:bg-(--theme-primary-eight)"
                        data-mavi-link-insert @onclick="InsertLinkAsync">Insert</button>
            </div>
        </div>
    }
</div>
```

- [ ] **Step 5: Add the Link icon**

```csharp
public static readonly string Link = Open +
    "<path d=\"M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71\"/>" +
    "<path d=\"M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71\"/>" + Close;
```

- [ ] **Step 6: Run tests → PASS.**

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor link dialog"
```

---

## Task 9: Image insert (file pick → base64)

**Files:** `WysiwygEditor.razor`, `WysiwygEditor.razor.cs`, `WysiwygIcons.cs`
**Test:** `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing test** (uses bUnit's `InputFileContent`)

```csharp
using Microsoft.AspNetCore.Components.Forms;
// ...
[Fact]
public async Task PickingImage_Embeds_Base64_Via_insertImage()
{
    JSInterop.Setup<string>("insertImage", _ => true).SetResult("");
    var cut = Render<WysiwygEditor>();

    var file = InputFileContent.CreateFromText("PNGDATA", "pic.png", contentType: "image/png");
    cut.FindComponent<InputFile>().UploadFiles(file);

    // allow the async read handler to complete
    cut.WaitForAssertion(() => JSInterop.Invocations.ContainsKey("insertImage").Should().BeTrue());

    JSInterop.Invocations["insertImage"].Last().Arguments[1].ToString()!
        .Should().StartWith("data:image/png;base64,");
}
```

> Add `using Bunit;` for `InputFileContent`. `UploadFiles` triggers the component's `OnChange`.

- [ ] **Step 2: Run to verify fail** — Expected: FAIL (no `InputFile` / handler).

- [ ] **Step 3: Add image parameters + handler to code-behind**

```csharp
using Microsoft.AspNetCore.Components.Forms;
// add field/param + handler:

[Parameter] public long MaxImageBytes { get; set; } = 5 * 1024 * 1024;

private async Task HandleImageSelectedAsync(InputFileChangeEventArgs e)
{
    var file = e.File;
    if (file is null || _jsModule is null || !IsInteractive) return;
    if (file.Size > MaxImageBytes) return;

    await using var stream = file.OpenReadStream(MaxImageBytes);
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);
    var base64 = Convert.ToBase64String(ms.ToArray());
    var dataUrl = $"data:{file.ContentType};base64,{base64}";

    var html = await _jsModule.InvokeAsync<string>("insertImage", _surfaceRef, dataUrl);
    await UpdateContentAsync(html);
}
```

- [ ] **Step 4: Add hidden `InputFile` + image button to `WysiwygEditor.razor`**

The image button clicks the hidden `InputFile`'s label. Add the button in the insert group and the input near the surface:

```razor
<div class="relative pr-1">
    <label class="@ToolbarButtonCss() cursor-pointer" title="Image">
        @((MarkupString)WysiwygIcons.Image)
        <InputFile OnChange="HandleImageSelectedAsync" accept="image/*"
                   class="hidden" data-mavi-image-input />
    </label>
</div>
```

> `InputFile` renders an `<input type="file">`; wrapping it in the `<label>` makes the whole button trigger the picker, and `class="hidden"` hides the native input.

- [ ] **Step 5: Add the Image icon**

```csharp
public static readonly string Image = Open +
    "<rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/>" +
    "<circle cx=\"8.5\" cy=\"8.5\" r=\"1.5\"/><polyline points=\"21 15 16 10 5 21\"/>" + Close;
```

- [ ] **Step 6: Run tests → PASS.**

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor base64 image insertion"
```

---

## Task 10: Table grid picker

**Files:** `WysiwygEditor.razor`, `WysiwygEditor.razor.cs`, `WysiwygIcons.cs`
**Test:** `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing test**

```csharp
[Fact]
public async Task TableGrid_Insert_Invokes_insertTable_WithDimensions()
{
    JSInterop.Setup<string>("insertTable", _ => true).SetResult("");
    var cut = Render<WysiwygEditor>();

    await cut.Find("[data-mavi-popover-toggle='table']").ClickAsync(new());
    await cut.Find("[data-mavi-table-cell='3x4']").ClickAsync(new());

    var inv = JSInterop.Invocations["insertTable"].Last();
    inv.Arguments[1].Should().Be(3); // rows
    inv.Arguments[2].Should().Be(4); // cols
}
```

- [ ] **Step 2: Run to verify fail** — Expected: FAIL.

- [ ] **Step 3: Add table state + handler**

```csharp
private const int TableGridMax = 8;
private int _tableHoverRows;
private int _tableHoverCols;

private void HoverTableCell(int rows, int cols)
{
    _tableHoverRows = rows;
    _tableHoverCols = cols;
}

private async Task InsertTableAsync(int rows, int cols)
{
    _openPopover = null;
    if (_jsModule is null || !IsInteractive) return;
    var html = await _jsModule.InvokeAsync<string>("insertTable", _surfaceRef, rows, cols);
    await UpdateContentAsync(html);
}
```

- [ ] **Step 4: Add table popover markup (8×8 hover grid)**

```razor
<div class="relative pr-1">
    <button type="button" class="@ToolbarButtonCss()" title="Table"
            data-mavi-popover-toggle="table" @onclick='() => TogglePopover("table")'>
        @((MarkupString)WysiwygIcons.Table)
    </button>
    @if (IsPopoverOpen("table"))
    {
        <div class="absolute left-0 top-full z-20 mt-1 rounded-md border border-(--theme-light-six) bg-white p-2 shadow-lg"
             data-mavi-popover="table">
            <div class="grid gap-0.5" style="grid-template-columns:repeat(@TableGridMax, 1rem)">
                @for (var r = 1; r <= TableGridMax; r++)
                {
                    var row = r;
                    @for (var c = 1; c <= TableGridMax; c++)
                    {
                        var col = c;
                        var active = row <= _tableHoverRows && col <= _tableHoverCols;
                        <button type="button"
                                class="@(active ? "h-4 w-4 border border-(--theme-primary-seven) bg-(--theme-primary-two)" : "h-4 w-4 border border-(--theme-light-six)")"
                                data-mavi-table-cell="@(row)x@(col)"
                                @onmouseover="() => HoverTableCell(row, col)"
                                @onclick="() => InsertTableAsync(row, col)"></button>
                    }
                }
            </div>
            <div class="mt-1 text-center text-xs text-(--theme-dark-six)">@_tableHoverRows × @_tableHoverCols</div>
        </div>
    }
</div>
```

- [ ] **Step 5: Add the Table icon**

```csharp
public static readonly string Table = Open +
    "<rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/>" +
    "<line x1=\"3\" y1=\"9\" x2=\"21\" y2=\"9\"/><line x1=\"3\" y1=\"15\" x2=\"21\" y2=\"15\"/>" +
    "<line x1=\"9\" y1=\"3\" x2=\"9\" y2=\"21\"/><line x1=\"15\" y1=\"3\" x2=\"15\" y2=\"21\"/>" + Close;
```

- [ ] **Step 6: Run tests → PASS.**

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor table grid picker"
```

---

## Task 11: Code view + fullscreen toggles

**Files:** `WysiwygEditor.razor`, `WysiwygEditor.razor.cs`, `WysiwygIcons.cs`
**Test:** `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
[Fact]
public async Task CodeView_Toggle_Shows_RawHtml_Textarea()
{
    JSInterop.Setup<string>("getHtml", _ => true).SetResult("<p>raw</p>");
    JSInterop.SetupVoid("setHtml", _ => true);
    var cut = Render<WysiwygEditor>();

    cut.FindAll("[data-mavi-wysiwyg-codearea]").Count.Should().Be(0);

    await cut.Find("[data-mavi-wysiwyg-codeview]").ClickAsync(new());
    var textarea = cut.Find("[data-mavi-wysiwyg-codearea]");
    textarea.GetAttribute("value").Should().Contain("<p>raw</p>");

    // toggling back pushes edited HTML into the surface
    await cut.Find("[data-mavi-wysiwyg-codeview]").ClickAsync(new());
    JSInterop.VerifyInvoke("setHtml");
}

[Fact]
public async Task Fullscreen_Toggle_Adds_FixedInset_Class()
{
    JSInterop.SetupVoid("toggleFullscreen", _ => true);
    var cut = Render<WysiwygEditor>();

    await cut.Find("[data-mavi-wysiwyg-fullscreen]").ClickAsync(new());

    cut.Find("[data-mavi-wysiwyg-root]").GetAttribute("class").Should().Contain("fixed");
}
```

- [ ] **Step 2: Run to verify fail** — Expected: FAIL.

- [ ] **Step 3: Add code-view + fullscreen state to code-behind**

```csharp
[Parameter] public bool ShowCodeViewButton { get; set; } = true;
[Parameter] public bool ShowFullscreenButton { get; set; } = true;

private bool _codeView;
private bool _fullscreen;
private string _codeHtml = string.Empty;

private async Task ToggleCodeViewAsync()
{
    if (!_codeView)
    {
        _codeHtml = await GetContentAsync();
        _codeView = true;
    }
    else
    {
        _codeView = false;
        await SetContentAsync(_codeHtml);
    }
}

private async Task ToggleFullscreenAsync()
{
    _fullscreen = !_fullscreen;
    if (_jsModule is not null)
    {
        await _jsModule.InvokeVoidAsync("toggleFullscreen", _fullscreen);
    }
}
```

Update `GetRootCss()` (from Task 1) to include the fullscreen classes:

```csharp
private string GetRootCss()
{
    var css = "rounded-md border border-(--theme-light-six) bg-white";
    if (_fullscreen)
    {
        css += " fixed inset-0 z-50 rounded-none";
    }
    if (Disabled)
    {
        css += " opacity-60 pointer-events-none";
    }
    return string.IsNullOrEmpty(Class) ? css : $"{css} {Class}";
}
```

> Move `GetRootCss()` from the `.razor` `@code` block into the `.razor.cs` (it now reads private state). Delete the duplicate in the `.razor`; keep `ToolbarButtonCss()` in the `.razor` `@code` (it is static/stateless) or move both — just avoid defining `GetRootCss` twice.

- [ ] **Step 4: Add code-view textarea + toggle buttons to `WysiwygEditor.razor`**

Add the two buttons at the end of the toolbar (after Save/Refresh):

```razor
@if (ShowCodeViewButton)
{
    <button type="button" class="@ToolbarButtonCss()" title="Code view"
            data-mavi-wysiwyg-codeview @onclick="ToggleCodeViewAsync">
        @((MarkupString)WysiwygIcons.Code)
    </button>
}
@if (ShowFullscreenButton)
{
    <button type="button" class="@ToolbarButtonCss()" title="Fullscreen"
            data-mavi-wysiwyg-fullscreen @onclick="ToggleFullscreenAsync">
        @((MarkupString)WysiwygIcons.Fullscreen)
    </button>
}
```

Wrap the surface so code view replaces it. Replace the surface `<div @ref…>` region with:

```razor
@if (_codeView)
{
    <textarea class="w-full bg-(--theme-dark-nine) p-4 font-mono text-sm text-(--theme-light-two) outline-none"
              style="min-height:@MinHeight"
              value="@_codeHtml"
              @oninput="e => _codeHtml = e.Value?.ToString() ?? string.Empty"
              data-mavi-wysiwyg-codearea></textarea>
}
<div @ref="_surfaceRef"
     class="w-full overflow-auto bg-white px-4 py-3 text-(--theme-dark-nine) outline-none resize-y focus:outline-none @(_codeView ? "hidden" : "")"
     style="min-height:@MinHeight"
     contenteditable="@IsInteractive.ToString().ToLowerInvariant()"
     role="textbox"
     aria-multiline="true"
     aria-label="@Title"
     data-placeholder="@Placeholder"
     data-mavi-wysiwyg-surface></div>
```

> The surface stays in the DOM (only visually hidden) so `_surfaceRef` remains valid for `setHtml` on toggle-back.

- [ ] **Step 5: Add Code + Fullscreen icons**

```csharp
public static readonly string Code = Open +
    "<polyline points=\"16 18 22 12 16 6\"/><polyline points=\"8 6 2 12 8 18\"/>" + Close;

public static readonly string Fullscreen = Open +
    "<path d=\"M8 3H5a2 2 0 0 0-2 2v3m18 0V5a2 2 0 0 0-2-2h-3m0 18h3a2 2 0 0 0 2-2v-3M3 16v3a2 2 0 0 0 2 2h3\"/>" + Close;
```

- [ ] **Step 6: Run tests → PASS.**

- [ ] **Step 7: Commit**

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "feat: add WysiwygEditor code view and fullscreen"
```

---

## Task 12: ReadOnly / Disabled behavior

**Files:** `WysiwygEditor.razor` (already gates toolbar via `hidden="@ReadOnly"` and surface via `IsInteractive`), `WysiwygEditor.razor.cs`
**Test:** `WysiwygEditorTests.cs`

- [ ] **Step 1: Write failing tests**

```csharp
[Fact]
public void ReadOnly_Hides_Toolbar_And_Locks_Surface()
{
    var cut = Render<WysiwygEditor>(p => p.Add(x => x.ReadOnly, true));

    cut.Find("[data-mavi-wysiwyg-toolbar]").HasAttribute("hidden").Should().BeTrue();
    cut.Find("[data-mavi-wysiwyg-surface]").GetAttribute("contenteditable").Should().Be("false");
}

[Fact]
public async Task Disabled_Prevents_Exec()
{
    JSInterop.Setup<string>("exec", _ => true).SetResult("");
    var cut = Render<WysiwygEditor>(p => p.Add(x => x.Disabled, true));

    // When Disabled the toolbar still renders (only ReadOnly hides it), but every
    // command handler guards on IsInteractive, so clicking Bold must not call exec.
    cut.Find("[data-mavi-wysiwyg-surface]").GetAttribute("contenteditable").Should().Be("false");
    await cut.Find("[data-mavi-cmd='bold']").ClickAsync(new());
    JSInterop.Invocations.ContainsKey("exec").Should().BeFalse();
}
```

> bUnit dispatches the click regardless of the `pointer-events-none` CSS, so this genuinely exercises the C# guard.

- [ ] **Step 2: Run to verify fail/pass**

Much of this may already pass from earlier tasks. Run the filter; if `Disabled` doesn't dim the root, continue.
Expected: at least the `Disabled_Prevents_Exec` assertion on `exec` not being called passes; adjust if `ReadOnly` toolbar hiding needs the `hidden` attribute (already added in Task 1).

- [ ] **Step 3: Ensure guards are complete**

Confirm every command handler begins with `if (_jsModule is null || !IsInteractive) return;` (added throughout Tasks 4–10). `ExecAsync`, `ApplyBlockStyleAsync`, `ApplyFontAsync`, `ApplyForeColorAsync`, `ApplyBackColorAsync`, `InsertLinkAsync`, `HandleImageSelectedAsync`, `InsertTableAsync` all include it. No code change if already present.

- [ ] **Step 4: Run tests → PASS.**

- [ ] **Step 5: Commit** (only if code changed)

```bash
git add src/Maviray.Blazor.Components.Material/Components/Editors/Wysiwyg tests/Maviray.Blazor.Components.Material.Tests/Components/Editors/Wysiwyg
git commit -m "test: cover WysiwygEditor readonly and disabled behavior"
```

---

## Task 13: Demo page + nav registration

**Files:**
- Create: `samples/Maviray.Blazor.Components.Samples.Material.Client/Pages/Editors/PageWysiwygEditor.razor`
- Modify: `samples/Maviray.Blazor.Components.Samples.Material.Client/Services/SampleCircuitStateService.cs`

- [ ] **Step 1: Create the demo page**

```razor
@page "/PageWysiwygEditor"
@using Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg

<h1 class="mb-4 text-2xl font-semibold">Wysiwyg Editor</h1>

<div class="max-w-3xl">
    <WysiwygEditor @ref="_editor"
                   @bind-Content="_html"
                   Placeholder="Start typing…"
                   ShowSaveButton="true"
                   ShowRefreshButton="true"
                   OnSave="OnSave"
                   RefreshContentProvider="LoadFromStore"
                   MinHeight="260px" />

    <div class="mt-4 flex gap-2">
        <button class="rounded bg-(--theme-primary-seven) px-3 py-1 text-white" @onclick="ShowHtml">Show HTML</button>
        <button class="rounded border px-3 py-1" @onclick="Clear">Clear</button>
    </div>

    @if (_lastSaved is not null)
    {
        <pre class="mt-4 overflow-auto rounded bg-(--theme-light-two) p-3 text-xs">@_lastSaved</pre>
    }
</div>

@code {
    private WysiwygEditor? _editor;
    private string? _html = "<p>Hello from <b>WysiwygEditor</b>!</p>";
    private string? _lastSaved;

    private Task OnSave(string html)
    {
        _lastSaved = html;
        return Task.CompletedTask;
    }

    private Task<string?> LoadFromStore() =>
        Task.FromResult<string?>("<p>Loaded from the provider callback.</p>");

    private async Task ShowHtml()
    {
        if (_editor is not null) _lastSaved = await _editor.GetContentAsync();
    }

    private async Task Clear()
    {
        if (_editor is not null) await _editor.ClearAsync();
    }
}
```

- [ ] **Step 2: Register the nav group**

In `SampleCircuitStateService.cs`, add a new `MenuItemGroup` to the `list` (e.g. after the "Feedback" group). Match the existing shape exactly:

```csharp
new MenuItemGroup
{
    Title = "Editors",
    Icon = "lni lni-pencil-1",
    BadgeColor = ThemeColorScheme.Primary,
    BadgeText = "1",
    Items =
    [
        new()
        {
            Title = "Wysiwyg Editor",
            Icon = "lni lni-pencil-1",
            NavigateTo = "PageWysiwygEditor"
        }
    ]
},
```

> If `lni lni-pencil-1` is not a valid glyph, reuse one already used in the file (e.g. `lni lni-colour-palette-3`). This only affects the nav icon.

- [ ] **Step 3: Build the samples project**

Run: `dotnet build samples/Maviray.Blazor.Components.Samples.Material.Client/Maviray.Blazor.Components.Samples.Material.Client.csproj`
Expected: Build succeeded.

- [ ] **Step 4: Commit**

```bash
git add samples/Maviray.Blazor.Components.Samples.Material.Client/Pages/Editors samples/Maviray.Blazor.Components.Samples.Material.Client/Services/SampleCircuitStateService.cs
git commit -m "feat: add WysiwygEditor samples demo page and nav entry"
```

---

## Task 14: Final polish, full build & test, manual verification

**Files:** any of the above (styling touch-ups)
**Test:** full suite

- [ ] **Step 1: Full build**

Run: `dotnet build Maviray.Blazor.Components.slnx`
Expected: Build succeeded, no warnings introduced by new files.

- [ ] **Step 2: Full test run**

Run: `dotnet test tests/Maviray.Blazor.Components.Material.Tests/Maviray.Blazor.Components.Material.Tests.csproj`
Expected: All tests pass (the original `MaviInputIntegerTests` + all `WysiwygEditorTests`).

- [ ] **Step 3: Manual smoke test in the samples app**

Run the samples server project and open `/PageWysiwygEditor`:
Run: `dotnet run --project samples/Maviray.Blazor.Components.Samples.Material/Maviray.Blazor.Components.Samples.Material.csproj`
Verify by hand: type text; Bold/Italic/Underline/Strikethrough/clear; Style dropdown H1/Quote/Code; font family; text + highlight color; both lists; alignment + indent/outdent; insert a link (opens new tab); insert an image via file pick (embeds, base64); insert a 3×3 table; Code view round-trips edits; Fullscreen fills the viewport and restores; Save shows HTML; Refresh loads provider text. Confirm **no video and no help** buttons exist.

- [ ] **Step 4: Verify the toolbar wraps at narrow width** — shrink the window; the toolbar should wrap to multiple rows (flex-wrap) without horizontal scroll.

- [ ] **Step 5: Commit any polish**

```bash
git add -A
git commit -m "polish: WysiwygEditor styling and final verification"
```

---

## Self-review notes (for the implementer)

- **Spec coverage:** Toolbar items 1–9 → Tasks 5,4,7,7,6,6,10,8+9,11. Content model (Content/Refresh/Save/get-set/clear/focus) → Task 3. Base64 images → Task 9. Code view/fullscreen → Task 11. ReadOnly/Disabled → Task 12. Demo page → Task 13. Video/help omitted (never added). Testing → every task. Styling/icons → per task + Task 14.
- **Type consistency:** JS functions are called with the surface element as the first argument everywhere (`exec(surface, …)`, `setHtml(surface, html)`, `insertTable(surface, rows, cols)`). C# handlers all read the returned HTML string and call `UpdateContentAsync`. Popover names are the literal strings `style`, `paragraph`, `font`, `color`, `link`, `table`.
- **Known follow-ups (out of scope):** active-state (`aria-pressed`) reflection on inline buttons; configurable toolbar groups; `MaviDialog` swap for the link popover if richer modal behavior is wanted.
