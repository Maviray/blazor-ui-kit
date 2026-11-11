using System.Reflection;

namespace Maviray.Blazor.Components.Material.Tests.TestUtilities;

/// <summary>
///     Counts the handlers attached to a field-like C# event. The compiler backs such an event with a
///     private delegate field of the same name, which lets tests detect handlers that were never detached.
/// </summary>
internal static class EventSubscriptions
{
    public static int Count(object source, string eventName)
    {
        var field = FindField(source.GetType(), eventName)
                    ?? throw new InvalidOperationException($"No backing field found for event '{eventName}' on {source.GetType().Name}.");

        return (field.GetValue(source) as Delegate)?.GetInvocationList().Length ?? 0;
    }

    private static FieldInfo? FindField(Type? type, string name)
    {
        while (type != null)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
            {
                return field;
            }

            type = type.BaseType;
        }

        return null;
    }
}
