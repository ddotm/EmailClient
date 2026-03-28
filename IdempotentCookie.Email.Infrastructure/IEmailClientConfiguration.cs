namespace IdempotentCookie.Email;

/// <summary>
/// Configuration contract for an email delivery provider.
/// </summary>
public interface IEmailClientConfiguration
{
  /// <summary>Gets the provider these settings apply to.</summary>
  EmailProvider Provider { get; }

  /// <summary>Validates the configuration, throwing if any required values are missing or invalid.</summary>
  void Validate();
}
