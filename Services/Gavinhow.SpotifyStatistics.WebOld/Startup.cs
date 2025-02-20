﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Gavinhow.SpotifyStatistics.Api;
using Gavinhow.SpotifyStatistics.Api.Settings;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Web.Services;
using Gavinhow.SpotifyStatistics.Web.Settings;
using Gavinhow.SpotifyStatistics.Web.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Gavinhow.SpotifyStatistics.Web
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(options =>
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

      string dbConnString = Configuration.GetConnectionString("Sql");
      string databaseType = Configuration.GetValue<string>("DatabaseType");
      services.AddDbContext<SpotifyStatisticsContext>(options =>
      {
        switch (databaseType.ToLower())
        {
          case "local":
            options.UseSqlite("DataSource=app.db");
            break;
          case "postgres":
            options.UseNpgsql(dbConnString);
            break;
          case "sqlserver":
            options.UseSqlServer(dbConnString);
            break;
          default:
            throw new ArgumentException("Unrecognised database type");
        }
      });

      services.AddControllers();
      services.AddMemoryCache(options => options.SizeLimit = 1024);
      services.Configure<SpotifySettings>(Configuration.GetSection("Spotify"));
      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
      var appSettingsSection = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSettingsSection);
      services.AddTransient<IUserService, UserService>();
      services.AddTransient<SpotifyApiFacade>();
      
      services
        .AddGraphQLServer()
        .AddQueryType<Query>();

      services.AddDistributedMemoryCache();

      // configure jwt authentication
      var appSettings = appSettingsSection.Get<AppSettings>();
      var key = Encoding.ASCII.GetBytes(appSettings.Secret);
      services.AddAuthentication(x =>
        {
          x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
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
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      if (env.IsProduction())
      {
        app.UseHttpsRedirection();
      }

      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseCors(MyAllowSpecificOrigins);
      app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

      using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        var context = serviceScope.ServiceProvider.GetService<SpotifyStatisticsContext>();
        if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
          context.Database.Migrate();
        }
      }
    }
  }
}