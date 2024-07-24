using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Publisher.OpenShift.Common.Configurations;

namespace Publisher.OpenShift.Domain.IoC
{
    public static class IoCExtension
    {
        public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var gitSettings = new GitSettings();
            var processSettings = new ProcessSettings();
            var openShiftSettings = new OpenShiftSettings();
            var packagesNugetSettings = new PackagesNugetSettings();
            
            configuration.Bind("GitSettings", gitSettings);
            configuration.Bind("ProcessSettings", processSettings);
            configuration.Bind("OpenShiftSettings", openShiftSettings);
            configuration.Bind("PackagesNugetSettings", packagesNugetSettings);
            
            services.AddSingleton(gitSettings);
            services.AddSingleton(processSettings);
            services.AddSingleton(openShiftSettings);
            services.AddSingleton(packagesNugetSettings);
        }
    }
}