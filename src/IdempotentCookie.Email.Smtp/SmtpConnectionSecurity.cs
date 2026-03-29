namespace IdempotentCookie.Email.Smtp;

/// <summary>
/// Controls how the SMTP connection negotiates transport security.
/// </summary>
public enum SmtpConnectionSecurity
{
    /// <summary>
    /// Let the SMTP client choose the best available security option.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Connect without TLS.
    /// </summary>
    None = 1,

    /// <summary>
    /// Connect over an SSL or TLS-secured socket immediately.
    /// </summary>
    SslOnConnect = 2,

    /// <summary>
    /// Connect in plain text and require STARTTLS.
    /// </summary>
    StartTls = 3,

    /// <summary>
    /// Connect in plain text and upgrade with STARTTLS when the server supports it.
    /// </summary>
    StartTlsWhenAvailable = 4
}