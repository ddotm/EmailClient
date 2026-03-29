using IdempotentCookie.Email;
using IdempotentCookie.Email.DependencyInjection;

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
        return builder.RegisterProvider(config, static configuration => new SmtpEmailClient(configuration));
    }
}