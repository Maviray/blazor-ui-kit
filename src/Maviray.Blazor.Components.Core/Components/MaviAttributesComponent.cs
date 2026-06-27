using Microsoft.AspNetCore.Components;

namespace Maviray.Blazor.Components.Core.Components;

public class MaviAttributesComponent : MaviComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = [];
}