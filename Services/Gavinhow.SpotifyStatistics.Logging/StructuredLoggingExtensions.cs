using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Gavinhow.SpotifyStatistics.Logging;

public static class StructuredLoggingExtensions
{
    public static WebApplicationBuilder AddStructuredLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        var serviceName = builder.Configuration["ServiceName"]
                          ?? builder.Configuration["SERVICE_NAME"]
                          ?? builder.Configuration["Service:Name"]
                          ?? builder.Environment.ApplicationName;

        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("service", serviceName)
                .Enrich.WithProperty("environment", context.HostingEnvironment.EnvironmentName)
                // Console sink only; logs are expected to be shipped by the runtime (Promtail/Fluent Bit/etc.).
                .WriteTo.Console(new JsonLogFormatter(serviceName, context.HostingEnvironment.EnvironmentName));
        });

        return builder;
    }

    public static WebApplication UseStructuredRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "Request {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("Method", httpContext.Request.Method);
                diagnosticContext.Set("Path", httpContext.Request.Path.Value ?? string.Empty);
                diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
                diagnosticContext.Set("Protocol", httpContext.Request.Protocol);
                diagnosticContext.Set("Scheme", httpContext.Request.Scheme);
                diagnosticContext.Set("Host", httpContext.Request.Host.Value);
            };
        });

        return app;
    }
}
