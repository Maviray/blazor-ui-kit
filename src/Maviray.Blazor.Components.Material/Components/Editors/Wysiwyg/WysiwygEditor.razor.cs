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
