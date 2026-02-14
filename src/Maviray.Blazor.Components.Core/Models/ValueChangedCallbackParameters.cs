namespace Maviray.Blazor.Components.Core.Models;

public class ValueChangedCallbackParameters<TValue>
{
    public string? ElementId { get; set; }

    public TValue? Value { get; set; }

}