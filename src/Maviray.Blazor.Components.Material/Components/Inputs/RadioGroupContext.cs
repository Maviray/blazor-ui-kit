using Maviray.Blazor.Components.Core.Enums;

namespace Maviray.Blazor.Components.Material.Components.Inputs;

public sealed class RadioGroupContext<TValue>
{
    public required string GroupId { get; init; }
    public required Func<TValue?> GetValue { get; init; }
    public required Func<TValue?, Task> SetValueAsync { get; init; }
    public required bool Disabled { get; init; }
    public required bool Readonly { get; init; }
    public required ElementSize ElementSize { get; init; }
    public required ThemeColorScheme ThemeColorScheme { get; init; }
}