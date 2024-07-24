using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Publisher.OpenShift.Domain.IoC;
using Serilog;

namespace Publisher.OpenShift
{
    public static class Startup
    {
        public static IHost AppStartup(string nomeConfiguracao)
        {
            var builder = new ConfigurationBuilder();
            AddEnvironmentVariables(builder, nomeConfiguracao);

            var configuration = builder.Build();
            SetLoggerConfiguration(configuration);

            return CreateDefaultBuilder(configuration);
        }

        private static void AddEnvironmentVariables(IConfigurationBuilder builder, string nomeConfiguracao)
        {
            builder.SetBasePath(GetBasePath())
                    .AddJsonFile($"appsettings{nomeConfiguracao}.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
        }

        private static void SetLoggerConfiguration(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"{GetBasePath()}\\logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static string GetBasePath()
        {
            var basePath = System.Configuration.ConfigurationManager.AppSettings["PastaAplicacao"];
            if (string.IsNullOrWhiteSpace(basePath))
                basePath = Directory.GetCurrentDirectory();

            return basePath;
        }

        private static IHost CreateDefaultBuilder(IConfiguration configuration)
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureServices((_, services) => {
                    services.AddFeatureManagement();
                    services.AddSettings(configuration);
                    services.AddServices();
                })
                .UseSerilog()
                .Build();
        }
    }
}