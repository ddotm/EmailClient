using IdempotentCookie.Email;
using IdempotentCookie.Email.DependencyInjection;

namespace IdempotentCookie.Email.SendGrid;

/// <summary>
/// Extends <see cref="IEmailSendingBuilder"/> with SendGrid provider registration.
/// </summary>
public static class SendGridEmailSendingBuilderExtensions
{
    /// <summary>
    /// Registers SendGrid as the active email provider.
    /// </summary>
    /// <param name="builder">The email sending builder.</param>
    /// <param name="config">SendGrid configuration. Validated at registration time.</param>
    /// <returns>The builder, for chaining.</returns>
    public static IEmailSendingBuilder UseSendGrid(this IEmailSendingBuilder builder, SendGridClientConfig config)
    {
        return builder.RegisterProvider(config, static configuration => new SendGridEmailClient(configuration));
    }
}