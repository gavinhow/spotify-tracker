using System.Configuration;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(Gavinhow.SpotifyStatistics.ImportFunction.Startup))]

namespace Gavinhow.SpotifyStatistics.ImportFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            string connectionString = config.GetConnectionString("MyConn");
            builder.Services.AddHttpClient();
            builder.Services.AddDbContext<SpotifyStatisticsContext>(options =>
                    options.UseNpgsql(connectionString))
                .AddOptions<SpotifySettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Spotify").Bind(settings);
                });
            builder.Services.AddTransient<SpotifyApiFacade, SpotifyApiFacade>();
        }
    }
}