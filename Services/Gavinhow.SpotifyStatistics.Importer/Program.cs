using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Importer;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
string dbConnString = builder.Configuration.GetConnectionString("Sql");
builder.Services.AddDbContext<SpotifyStatisticsContext>(options => { options.UseNpgsql(dbConnString); });

builder.Services.AddMemoryCache(options => options.SizeLimit = 1024);
builder.Services.Configure<SpotifySettings>(builder.Configuration.GetSection("Spotify"));
builder.Services.AddTransient<SpotifyApiFacade>();
builder.Services.AddScoped<SpotifyImporter>();

var host = builder.Build();
host.Run();