using CoffeeBeans.backend.Web.Domain.Interfaces;
using CoffeeBeans.backend.Web.Infrastructure;
using CoffeeBeans.backend.Web.Infrastructure.Email;

namespace CoffeeBeans.backend.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {
    var allowedOrigins = builder.Configuration
      .GetSection("Cors:AllowedOrigins")
      .Get<string[]>()
      ?? ["http://localhost:3000", "https://localhost:3000"];

    services.AddCors(options =>
    {
      options.AddDefaultPolicy(policy =>
      {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
      });
    });

    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatorSourceGen(logger);

    if (builder.Environment.IsDevelopment())
    {
      // Use a local test email server
      // See: https://ardalis.com/configuring-a-local-test-email-server/
      services.AddScoped<IEmailSender, MimeKitEmailSender>();

      // Otherwise use this:
      //builder.Services.AddScoped<IEmailSender, FakeEmailSender>();

    }
    else
    {
      services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }

    logger.LogInformation("{Project} services registered", "Mediator and Email Sender");

    return services;
  }


}
