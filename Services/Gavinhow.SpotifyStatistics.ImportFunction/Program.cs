using Azure.Data.Tables;
using Azure.Storage.Queues;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;
        // Application Insights — connection string is read automatically from
        // the APPLICATIONINSIGHTS_CONNECTION_STRING env var set in Azure.
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.Configure<LoggerFilterOptions>(options =>
        {
            // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
            // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
            LoggerFilterRule? toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
                                                                              == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

            if (toRemove is not null)
            {
                options.Rules.Remove(toRemove);
            }
        });
        // Spotify API
        services.Configure<SpotifySettings>(config.GetSection("Spotify"));
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddTransient<SpotifyApiFacade>();

        // Azure Storage — shared connection string for both queue and table
        var storageConnectionString = config["AzureStorageConnection"];

        var queueName = config["PlaysQueueName"] ?? "spotify-plays";
        services.AddSingleton(new QueueClient(storageConnectionString, queueName));

        var tableName = config["UsersTableName"] ?? "Users";
        services.AddSingleton(new TableClient(storageConnectionString, tableName));
    })
    .Build();

host.Run();
