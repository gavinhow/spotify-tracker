﻿using System;
using System.IO;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Backend.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            string dbConnString = Configuration.GetConnectionString("Sql");
            Console.WriteLine(dbConnString);


            var hostBuilder = new HostBuilder()
           // Add configuration, logging, ...
           .ConfigureServices((hostContext, services) =>
           {
               services.AddDbContext<SpotifyStatisticsContext>(options =>
                    options.UseSqlServer(dbConnString))
                    .Configure<SpotifySettings>(Configuration.GetSection("Spotify"))
                    .AddLogging(configure =>
                        configure.AddConsole()
                                .SetMinimumLevel(LogLevel.Trace))
                    .AddTransient<SpotifyApi, SpotifyApi>()
                    .AddHostedService<ImportHostedService>();
           });

            await hostBuilder.RunConsoleAsync();
        }
    }
}
