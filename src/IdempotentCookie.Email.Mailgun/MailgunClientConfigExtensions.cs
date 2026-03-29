using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Mailgun;

/// <summary>
/// Creates Mailgun-backed email clients without a dependency injection container.
/// </summary>
public static class MailgunClientConfigExtensions
{
    /// <summary>
    /// Creates an email client for the specified Mailgun configuration.
    /// </summary>
    /// <param name="config">The Mailgun configuration.</param>
    /// <returns>A configured <see cref="IEmailClient"/>.</returns>
    public static IEmailClient CreateClient(this MailgunClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        return new MailgunClient(config);
    }
}