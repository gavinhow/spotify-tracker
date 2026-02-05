using System.Reflection;
using System.Text;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Services;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Authorization.Handler;
using Gavinhow.SpotifyStatistics.Web.Authorization.Requirements;
using Gavinhow.SpotifyStatistics.Web.BackgroundServices;
using Gavinhow.SpotifyStatistics.Web.Extensions;
using Gavinhow.SpotifyStatistics.Web.Observability;
using Gavinhow.SpotifyStatistics.Logging;
using Gavinhow.SpotifyStatistics.Web.Services;
using Gavinhow.SpotifyStatistics.Web.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using IAuthorizationHandler = Microsoft.AspNetCore.Authorization.IAuthorizationHandler;

var builder = WebApplication.CreateBuilder(args);

builder.AddStructuredLogging();

builder.Services.UseHttpClientMetrics();

var configuredServiceName = builder.Configuration["ServiceName"]
    ?? builder.Configuration["SERVICE_NAME"]
    ?? builder.Configuration["Service:Name"]
    ?? "spotify-api";

builder.Services.Configure<MetricsSettings>(builder.Configuration.GetSection("Metrics"));
var metricsSettings = builder.Configuration.GetSection("Metrics").Get<MetricsSettings>() ?? new MetricsSettings();
metricsSettings.ServiceName = string.IsNullOrWhiteSpace(metricsSettings.ServiceName)
    ? configuredServiceName
    : metricsSettings.ServiceName;

builder.Services.AddSingleton(new QueueProcessingMetrics(metricsSettings.ServiceName));

var corsSettings = builder.Configuration.GetSection("Cors").Get<CorsSettings>();
if (corsSettings == null || corsSettings.AllowedOrigins.Length == 0)
{
  throw new InvalidOperationException("CORS configuration is missing or has no allowed origins");
}

builder.Services.AddCors(options =>
{
  options.AddPolicy("default", policy =>
  {
    policy.WithOrigins(corsSettings.AllowedOrigins);
    policy.WithMethods(corsSettings.AllowedMethods);
    policy.WithHeaders(corsSettings.AllowedHeaders);

    if (corsSettings.ExposedHeaders.Length > 0)
    {
      policy.WithExposedHeaders(corsSettings.ExposedHeaders);
    }

    policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.MaxAgeSeconds));

    if (corsSettings.AllowCredentials)
    {
      policy.AllowCredentials();
    }
  });
});
string dbConnString = builder.Configuration.GetConnectionString("Sql");
builder.Services.AddDbContext<SpotifyStatisticsContext>(options => { options.UseNpgsql(dbConnString); });

builder.Services.ConfigureQueryTypes(Assembly.GetExecutingAssembly());
var appSettingsSection = builder.Configuration.GetSection("AppSettings");

// configure jwt authentication
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(x =>
  {
    x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(key),
      ValidateIssuer = true,
      ValidIssuer = appSettings.Issuer,
      ValidateAudience = true,
      ValidAudience = appSettings.Audience,
      ClockSkew = TimeSpan.FromMinutes(5)
    };
  });

builder.Services.AddAuthentication("ApiKey")
  .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", x =>
  {
    x.ApiKey = builder.Configuration["DEMO_API_KEY"] ?? throw new InvalidOperationException();
  });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ApiKeyOrBearer", policy =>
  {
    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "ApiKey");
    policy.RequireAuthenticatedUser();
  }).AddPolicy(AllowedUserIdRequirement.AllowedUserIdPolicy, policy => policy.Requirements.Add(new AllowedUserIdRequirement()));

builder.Services.AddControllers();
builder.Services.AddMemoryCache(options => options.SizeLimit = 1024);
builder.Services.Configure<ApiKeyAuthenticationOptions>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.Configure<SpotifySettings>(builder.Configuration.GetSection("Spotify"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<ImportSettings>(builder.Configuration.GetSection("Import"));
builder.Services.Configure<AppSettings>(appSettingsSection);
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<SpotifyApiFacade>();
builder.Services.AddScoped<IImportStatusService, ImportStatusService>();
builder.Services.AddScoped<IAuthorizationHandler, AllowedUserIdHandler>();

// Queue consumer configuration
builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection("Queue"));
var queueSettings = builder.Configuration.GetSection("Queue").Get<QueueSettings>();

if (queueSettings?.Enabled == true && !string.IsNullOrEmpty(queueSettings.ConnectionString))
{
    builder.Services.AddSingleton(new QueueClient(queueSettings.ConnectionString, queueSettings.QueueName));
    builder.Services.AddScoped<IPlaysQueueProcessor, PlaysQueueProcessor>();
    builder.Services.AddHostedService<QueueConsumerBackgroundService>();
}

// Table Storage — user-token resilient store + periodic Postgres → Azure sync
builder.Services.Configure<TableStorageSettings>(builder.Configuration.GetSection("TableStorage"));
var tableStorageSettings = builder.Configuration.GetSection("TableStorage").Get<TableStorageSettings>();

if (tableStorageSettings?.Enabled == true && !string.IsNullOrEmpty(tableStorageSettings.ConnectionString))
{
    builder.Services.AddSingleton(new TableClient(tableStorageSettings.ConnectionString, tableStorageSettings.TableName));
    builder.Services.AddScoped<IUserTokenStore, UserTokenStore>();
    builder.Services.AddHostedService<UserTokenSyncBackgroundService>();
}
else
{
    builder.Services.AddScoped<IUserTokenStore, NoOpUserTokenStore>();
}

builder
  .AddGraphQL()
  .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
  .AddAuthorization()
  .AddTypes()
  .AddFiltering()
  .AddProjections();

builder.Services.AddDistributedMemoryCache();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SpotifyStatisticsContext>("database")
    .ForwardToPrometheus();

var app = builder.Build();

app.UseStructuredRequestLogging();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.MapNitroApp();
}

app.UseCors("default");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQLHttp().RequireAuthorization("ApiKeyOrBearer");
app.MapControllers();
app.MapMetrics(metricsSettings.Path).RequireHost(metricsSettings.AllowedHost);
// Map health check endpoint
app.MapHealthChecks("/health");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
  var context = serviceScope.ServiceProvider.GetService<SpotifyStatisticsContext>();
  if (context?.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
  {
    context?.Database.Migrate();
  }
}

app.RunWithGraphQLCommands(args);
