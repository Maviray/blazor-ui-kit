using System.Globalization;

namespace Maviray.Blazor.Components.Material.Tests.TestUtilities;

/// <summary>
///     Sets the current formatting and UI cultures for the duration of a test and restores them afterwards.
/// </summary>
internal sealed class CultureScope : IDisposable
{
    private readonly CultureInfo _previousCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _previousUiCulture = CultureInfo.CurrentUICulture;

    public CultureScope(string culture, string uiCulture)
    {
        CultureInfo.CurrentCulture = new(culture);
        CultureInfo.CurrentUICulture = new(uiCulture);
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _previousCulture;
        CultureInfo.CurrentUICulture = _previousUiCulture;
    }
}
