using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace IdempotentCookie.Email.DependencyInjection;

/// <summary>
/// Extension methods for registering email sending services on <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the email sending infrastructure and returns a builder for provider selection.
    /// </summary>
    /// <example>
    /// services.AddEmailSending().UseMailgun(config);
    /// </example>
    public static IEmailSendingBuilder AddEmailSending(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, EmailSendingStartupValidationHostedService>());

        return new EmailSendingBuilder(services);
    }
}
