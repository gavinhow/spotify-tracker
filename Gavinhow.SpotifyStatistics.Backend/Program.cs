using System;
using System.IO;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Backend.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gavinhow.SpotifyStatistics.Backend
{
    class Program
    {
        //public static ILoggerFactory LoggerFactory;
        public static IConfigurationRoot Configuration;


        async static Task Main(string[] args)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddApplicationInsightsSettings();

            Configuration = builder.Build();
            string dbConnString = Configuration.GetConnectionString("Sql");
            Console.WriteLine(dbConnString);

            var appInsightsSettings = new ApplicationInsightsSettings()
            {
                InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"],
                DeveloperMode = bool.Parse(Configuration["ApplicationInsights:DeveloperMode"])
            };

            var hostBuilder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<SpotifyStatisticsContext>(options =>
                     options.UseSqlServer(dbConnString))
                     .Configure<SpotifySettings>(Configuration.GetSection("Spotify"))
                     .AddLogging(configure =>
                         configure.AddConsole()
                                 .SetMinimumLevel(LogLevel.Information)
                         .AddProvider(new ApplicationInsightsLoggerProvider(appInsightsSettings)))
                     .AddTransient<SpotifyApi, SpotifyApi>()
                     .AddHostedService<ImportHostedService>()
                     .Configure<ApplicationInsightsSettings>(Configuration.GetSection("ApplicationInsights"))
                     .AddApplicationInsightsTelemetry(Configuration);
            });

            var host = hostBuilder.UseConsoleLifetime().Build();

            await host.RunAsync();
        }
    }
}
