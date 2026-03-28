using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Smtp;

/// <summary>
/// Creates SMTP-backed email clients without a dependency injection container.
/// </summary>
public static class SmtpClientConfigExtensions
{
    /// <summary>
    /// Creates an email client for the specified SMTP configuration.
    /// </summary>
    /// <param name="config">The SMTP configuration.</param>
    /// <returns>A configured <see cref="IEmailClient"/>.</returns>
    public static IEmailClient CreateClient(this SmtpClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        return new SmtpEmailClient(config);
    }
}