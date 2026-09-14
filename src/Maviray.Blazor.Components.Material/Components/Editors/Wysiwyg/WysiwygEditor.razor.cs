using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

public partial class WysiwygEditor : IAsyncDisposable
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

    #region Save / Refresh parameters

    [Parameter] public bool ShowSaveButton { get; set; }
    [Parameter] public EventCallback<string> OnSave { get; set; }
    [Parameter] public bool ShowRefreshButton { get; set; }
    [Parameter] public Func<Task<string?>>? RefreshContentProvider { get; set; }

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
