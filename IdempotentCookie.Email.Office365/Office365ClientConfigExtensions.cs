using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Office365;

/// <summary>
/// Creates Office365-backed email clients without a dependency injection container.
/// </summary>
public static class Office365ClientConfigExtensions
{
    /// <summary>
    /// Creates an email client for the specified Office365 configuration.
    /// </summary>
    /// <param name="config">The Office365 configuration.</param>
    /// <returns>A configured <see cref="IEmailClient"/>.</returns>
    public static IEmailClient CreateClient(this Office365ClientConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        return new Office365EmailClient(config);
    }
}