namespace IdempotentCookie.Email;

/// <summary>
/// Identifies the email delivery provider.
/// </summary>
public enum EmailProvider
{
  /// <summary>Provider is not set.</summary>
  Unknown = 0,
  /// <summary>SMTP delivery.</summary>
  Smtp = 1,
  /// <summary>Mailgun delivery.</summary>
  Mailgun = 2,
  /// <summary>SendGrid delivery.</summary>
  SendGrid = 3,
  /// <summary>Microsoft Office 365 delivery.</summary>
  Office365 = 4
}
