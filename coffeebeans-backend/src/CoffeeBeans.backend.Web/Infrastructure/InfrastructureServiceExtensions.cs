using Ardalis.GuardClauses;
using CoffeeBeans.backend.Web.BeanFeatures.List;
using CoffeeBeans.backend.Web.BeanFeatures.Add;
using CoffeeBeans.backend.Web.Infrastructure.Data;
using CoffeeBeans.backend.Web.Infrastructure.Data.Commands;
using CoffeeBeans.backend.Web.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeans.backend.Web.Infrastructure;

public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    var connectionString = config.GetConnectionString("AppDb");
    Guard.Against.NullOrWhiteSpace(connectionString, message: "Connection string 'AppDb' is required.");

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();

      options.UseMySQL(connectionString, sqlOptions =>
      {
        sqlOptions.EnableRetryOnFailure(
          maxRetryCount: 5,
          maxRetryDelay: TimeSpan.FromSeconds(10),
          errorNumbersToAdd: null);
      });

      options.AddInterceptors(eventDispatchInterceptor);
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
    services.AddScoped<IListBeansQueryService, ListBeansQueryService>();
    services.AddScoped<IAddColumnService, AddColumnService>();

    logger.LogInformation("Infrastructure services registered with MySQL.");

    return services;
  }
}
