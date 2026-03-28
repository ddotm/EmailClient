using IdempotentCookie.Email;
using IdempotentCookie.Email.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdempotentCookie.Email.Smtp;

/// <summary>
/// Extends <see cref="IEmailSendingBuilder"/> with SMTP provider registration.
/// </summary>
public static class SmtpEmailSendingBuilderExtensions
{
    /// <summary>
    /// Registers the generic SMTP provider as the active email provider.
    /// </summary>
    /// <param name="builder">The email sending builder.</param>
    /// <param name="config">SMTP configuration. Validated at registration time.</param>
    /// <returns>The builder, for chaining.</returns>
    public static IEmailSendingBuilder UseSmtp(this IEmailSendingBuilder builder, SmtpClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(config);

        config.Validate();

        builder.Services.RemoveAll<IEmailClient>();
        builder.Services.RemoveAll<IEmailClientConfiguration>();
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<IEmailClientConfiguration>(config);
        builder.Services.AddSingleton<IEmailClient>(_ => new SmtpEmailClient(config));

        return builder;
    }
}