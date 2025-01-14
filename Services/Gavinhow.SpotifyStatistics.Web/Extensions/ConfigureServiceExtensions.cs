using System.Reflection;
using Gavinhow.SpotifyStatistics.Web.CQRS;
using Gavinhow.SpotifyStatistics.Web.Queries;

namespace Gavinhow.SpotifyStatistics.Web.Extensions;

public static class ConfigureServiceExtensions
{
  public static IServiceCollection ConfigureQueryTypes(this IServiceCollection services, Assembly assembly)
  {
    services
      .ConfigureQueries(assembly);

    return services;
  }
  
  private static IServiceCollection ConfigureQueries(this IServiceCollection services, Assembly assembly)
  {
    var queryTypes = assembly.GetTypes()
      .Where(
        t => t.GetInterfaces()
          .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<,>)))
      .ToList();

    foreach (var queryType in queryTypes)
    {
      var interfaceType = queryType.GetInterfaces()
        .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<,>));

      services.AddTransient(interfaceType, queryType);
    }

    return services;
  }
}