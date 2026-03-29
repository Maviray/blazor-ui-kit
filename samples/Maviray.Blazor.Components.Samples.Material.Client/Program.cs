using Maviray.Blazor.Components.Core.Extensions;
using Maviray.Blazor.Components.Core.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Maviray.Blazor.Components.Samples.Material.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Configure Maviray components
            builder.Services.AddMaviComponents(options => options.EnableLifecycleLogging = false);
            builder.Services.AddScoped<ICircuitStateService, CircuitStateService>();

            await builder.Build().RunAsync();
        }
    }
}
