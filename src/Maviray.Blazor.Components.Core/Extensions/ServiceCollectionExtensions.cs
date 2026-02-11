using Maviray.Blazor.Components.Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Maviray.Blazor.Components.Core.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds Maviray Blazor components with default configuration.
        /// </summary>
        public IServiceCollection AddMaviComponents()
        {
            return services.AddMaviComponents(options => { });
        }

        /// <summary>
        /// Adds Maviray Blazor components with custom configuration.
        /// </summary>
        public IServiceCollection AddMaviComponents(Action<MaviComponentOptions> configure)
        {
            var options = new MaviComponentOptions();
            configure(options);

            services.AddSingleton<IMaviComponentOptions>(options);

            return services;
        }
    }
}