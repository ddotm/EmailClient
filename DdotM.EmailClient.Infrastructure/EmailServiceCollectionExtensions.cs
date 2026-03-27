using Microsoft.Extensions.DependencyInjection;

namespace DdotM.EmailClient.Infrastructure;

public static class EmailServiceCollectionExtensions
{
  public static IServiceCollection AddEmailSupport<TConfiguration>(
      this IServiceCollection services,
      TConfiguration configuration)
      where TConfiguration : class, IEmailClientConfiguration
  {
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configuration);

    configuration.Validate();

    services.AddSingleton(configuration);
    services.AddSingleton<IEmailClientConfiguration>(configuration);

    return services;
  }
}
