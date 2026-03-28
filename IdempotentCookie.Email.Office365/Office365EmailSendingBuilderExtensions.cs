using IdempotentCookie.Email;
using IdempotentCookie.Email.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdempotentCookie.Email.Office365;

/// <summary>
/// Extends <see cref="IEmailSendingBuilder"/> with Office365 provider registration.
/// </summary>
public static class Office365EmailSendingBuilderExtensions
{
    /// <summary>
    /// Registers Office365 as the active email provider.
    /// </summary>
    /// <param name="builder">The email sending builder.</param>
    /// <param name="config">Office365 configuration. Validated at registration time.</param>
    /// <returns>The builder, for chaining.</returns>
    public static IEmailSendingBuilder UseOffice365(this IEmailSendingBuilder builder, Office365ClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(config);

        config.Validate();

        builder.Services.RemoveAll<IEmailClient>();
        builder.Services.RemoveAll<IEmailClientConfiguration>();
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<IEmailClientConfiguration>(config);
        builder.Services.AddSingleton<IEmailClient>(_ => new Office365EmailClient(config));

        return builder;
    }
}