namespace IdempotentCookie.Email;

/// <summary>
/// Abstraction for an email delivery client bound to a specific provider.
/// </summary>
public interface IEmailClient
{
  /// <summary>Gets the provider this client delivers through.</summary>
  EmailProvider Provider { get; }

  /// <summary>Sends the specified message.</summary>
  Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
