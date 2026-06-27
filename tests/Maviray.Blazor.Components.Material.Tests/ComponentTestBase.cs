using Maviray.Blazor.Components.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Maviray.Blazor.Components.Material.Tests;

/// <summary>
///     Base class for all component tests that provides common setup and services.
/// </summary>
public abstract class ComponentTestBase : BunitContext
{
    protected ComponentTestBase()
    {
        // Register required services for all Maviray components
        Services.AddMaviComponents();

        Services.AddLogging();
    }
}