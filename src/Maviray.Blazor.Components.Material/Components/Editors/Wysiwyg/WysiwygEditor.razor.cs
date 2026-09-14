using Maviray.Blazor.Components.Core.Constants;
using Maviray.Blazor.Components.Core.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Maviray.Blazor.Components.Material.Components.Editors.Wysiwyg;

public partial class WysiwygEditor : IAsyncDisposable
{
    private const string JsModulePath =
        "./_content/Maviray.Blazor.Components.Material/js/maviWysiwygJsInterop.js";

    private ElementReference _surfaceRef;
    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<WysiwygEditor>? _dotNetRef;
    private string? _openPopover;
    private string _linkUrl = string.Empty;
    private string _linkText = string.Empty;

    private const int TableGridMax = 8;
    private int _tableHoverRows;
    private int _tableHoverCols;

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

    #region Code view / fullscreen parameters

    [Parameter] public bool ShowCodeViewButton { get; set; } = true;
    [Parameter] public bool ShowFullscreenButton { get; set; } = true;

    private bool _codeView;
    private bool _fullscreen;
    private string _codeHtml = string.Empty;

    #endregion

    #region Font / color parameters

    [Parameter] public IEnumerable<string>? FontFamilies { get; set; }
    [Parameter] public IEnumerable<string>? ColorPalette { get; set; }

    private IEnumerable<string> EffectiveFonts => FontFamilies ?? WysiwygDefaults.Fonts;
    private IEnumerable<string> EffectiveColors => ColorPalette ?? WysiwygDefaults.Colors;
    private string _currentFont = "Helvetica";

    #endregion

    #region Image parameters

    [Parameter] public long MaxImageBytes { get; set; } = 5 * 1024 * 1024;

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

        await JsRuntime.InvokeVoidAsync(
            JsInteropConstants.REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, Id, _dotNetRef, nameof(HandleOutsideClick));
    }

    private bool IsInteractive => !ReadOnly && !Disabled;

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
        if (string.Equals(Content, html, StringComparison.Ordinal))
        {
            return;
        }

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
            await _jsModule.InvokeVoidAsync("toggleFullscreen", _surfaceRef, _fullscreen);
        }
    }

    private async Task ExecAsync(string command, string? value = null)
    {
        if (_jsModule is null || !IsInteractive) return;
        var html = await _jsModule.InvokeAsync<string>("exec", _surfaceRef, command, value);
        await UpdateContentAsync(html);
    }

    private async Task ApplyBlockStyleAsync(string tag)
    {
        _openPopover = null;
        if (_jsModule is null || !IsInteractive) return;
        var html = await _jsModule.InvokeAsync<string>("formatBlock", _surfaceRef, tag);
        await UpdateContentAsync(html);
    }

    private async Task ExecFromPopoverAsync(string command)
    {
        _openPopover = null;
        await ExecAsync(command);
    }

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

    private async Task InsertLinkAsync()
    {
        _openPopover = null;
        if (_jsModule is null || !IsInteractive || string.IsNullOrWhiteSpace(_linkUrl)) return;
        var html = await _jsModule.InvokeAsync<string>("insertLink", _surfaceRef, _linkUrl, _linkText);
        await UpdateContentAsync(html);
        _linkUrl = string.Empty;
        _linkText = string.Empty;
    }

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

    private async Task HandleImageSelectedAsync(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file is null || _jsModule is null || !IsInteractive) return;
        if (file.Size > MaxImageBytes) return;

        try
        {
            await using var stream = file.OpenReadStream(MaxImageBytes);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var dataUrl = $"data:{file.ContentType};base64,{base64}";

            var html = await _jsModule.InvokeAsync<string>("insertImage", _surfaceRef, dataUrl);
            await UpdateContentAsync(html);
        }
        catch (IOException ex)
        {
            Logger?.Error(ex, "WysiwygEditor: failed to read selected image.");
        }
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (JsRuntime is not null)
            {
                await JsRuntime.InvokeVoidAsync(JsInteropConstants.UN_REGISTER_OUT_OF_FOCUS_CALLBACK_LISTENER, Id);
            }
        }
        catch (JSDisconnectedException) { /* circuit gone */ }

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
