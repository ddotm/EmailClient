using IdempotentCookie.Email;
using IdempotentCookie.Email.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(config);

        config.Validate();

        builder.Services.RemoveAll<IEmailClient>();
        builder.Services.RemoveAll<IEmailClientConfiguration>();
        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<IEmailClientConfiguration>(config);
        builder.Services.AddSingleton<IEmailClient>(_ => new SendGridEmailClient(config));

        return builder;
    }
}