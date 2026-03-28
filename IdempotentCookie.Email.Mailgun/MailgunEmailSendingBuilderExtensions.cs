using IdempotentCookie.Email;
using IdempotentCookie.Email.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace IdempotentCookie.Email.Mailgun;

/// <summary>
/// Extends <see cref="IEmailSendingBuilder"/> with Mailgun provider registration.
/// </summary>
public static class MailgunEmailSendingBuilderExtensions
{
    /// <summary>
    /// Registers Mailgun as the active email provider.
    /// </summary>
    /// <param name="builder">The email sending builder.</param>
    /// <param name="config">Mailgun configuration. Validated at registration time.</param>
    /// <returns>The builder, for chaining.</returns>
    public static IEmailSendingBuilder UseMailgun(this IEmailSendingBuilder builder, MailgunClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(config);

        config.Validate();

        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<IEmailClient>(_ => new MailgunEmailClient(config));

        return builder;
    }
}
