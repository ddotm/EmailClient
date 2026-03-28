using IdempotentCookie.Email;

namespace IdempotentCookie.Email.SendGrid;

/// <summary>
/// Creates SendGrid-backed email clients without a dependency injection container.
/// </summary>
public static class SendGridClientConfigExtensions
{
    /// <summary>
    /// Creates an email client for the specified SendGrid configuration.
    /// </summary>
    /// <param name="config">The SendGrid configuration.</param>
    /// <returns>A configured <see cref="IEmailClient"/>.</returns>
    public static IEmailClient CreateClient(this SendGridClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        return new SendGridEmailClient(config);
    }
}