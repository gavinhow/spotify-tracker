using System.Reflection;
using System.Text;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Authorization.Handler;
using Gavinhow.SpotifyStatistics.Web.Authorization.Requirements;
using Gavinhow.SpotifyStatistics.Web.Extensions;
using Gavinhow.SpotifyStatistics.Web.Services;
using Gavinhow.SpotifyStatistics.Web.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using IAuthorizationHandler = Microsoft.AspNetCore.Authorization.IAuthorizationHandler;

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole(options =>
{
  options.IncludeScopes = false;
});

builder.Services.AddCors(options =>
{
  options.AddPolicy(MyAllowSpecificOrigins,
    builder =>
    {
      builder.AllowAnyOrigin();
      // builder.AllowCredentials();
      builder.AllowAnyHeader();
      builder.AllowAnyMethod();
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
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(key),
      ValidateIssuer = false,
      ValidateAudience = false
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
builder.Services.Configure<AppSettings>(appSettingsSection);
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<SpotifyApiFacade>();
builder.Services.AddScoped<IAuthorizationHandler, AllowedUserIdHandler>();

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
    .AddDbContextCheck<SpotifyStatisticsContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.MapNitroApp();
}

// if (app.Environment.IsProduction())
// {
//   app.UseHttpsRedirection();
// }

app.UseCors(MyAllowSpecificOrigins);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQLHttp().RequireAuthorization("ApiKeyOrBearer");
app.MapControllers();

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