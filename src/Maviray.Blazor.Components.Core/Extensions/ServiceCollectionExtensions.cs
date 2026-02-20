using Maviray.Blazor.Components.Core.Options;
using Maviray.Blazor.Components.Core.Services;
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
            return services.AddMaviComponents(configure, _ => { });
        }

        /// <summary>
        /// Adds Maviray Blazor components with custom configuration for both general and toastr options.
        /// </summary>
        public IServiceCollection AddMaviComponents(Action<MaviComponentOptions> configure, Action<MaviToastrOptions> configureToastr)
        {
            var options = new MaviComponentOptions();
            configure(options);

            var toastrOptions = new MaviToastrOptions();
            configureToastr(toastrOptions);

            services.AddSingleton<IMaviComponentOptions>(options);
            services.AddSingleton(toastrOptions);
            services.AddSingleton<IMaviToastrService, MaviToastrService>();

            return services;
        }
    }
}