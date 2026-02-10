using Maviray.Blazor.Components.Core.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Maviray.Blazor.Components.Samples.Material.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Configure Maviray components
            builder.Services.AddMaviComponents(options =>
            {
                options.ComponentLogLevel = LogLevel.Debug;
                options.EnableLifecycleLogging = true;
                options.EnablePerformanceTracking = true;
            });

            await builder.Build().RunAsync();
        }
    }
}
