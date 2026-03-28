using IdempotentCookie.Email;
using IdempotentCookie.Email.DependencyInjection;

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
        return builder.RegisterProvider(config, static configuration => new Office365EmailClient(configuration));
    }
}