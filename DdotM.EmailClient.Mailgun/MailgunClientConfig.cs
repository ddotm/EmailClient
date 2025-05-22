using System.ComponentModel.DataAnnotations;

namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Configures Mailgun client with API key, sending domain, and TLS settings.
/// </summary>
public class MailgunClientConfig
{
    public readonly string ApiUser = "api";

    /// <summary>
    /// Mailgun API key. Private key from https://app.mailgun.com/app/account/security/api_keys
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Mailgun sending domain
    /// </summary>
    [Required]
    public string SendingDomain { get; set; } = string.Empty;

    /// <summary>
    /// If set to True this requires the message only be sent over a TLS connection.
    /// If a TLS connection can not be established, Mailgun will not deliver the message.
    /// Defaults to true
    /// </summary>
    public bool RequireTls { get; set; } = true;

    /// <summary>
    /// If set to True, the certificate and hostname will not be verified when trying to establish a TLS connection and
    /// Mailgun will accept any certificate during delivery.
    /// If set to False, Mailgun will verify the certificate and hostname.
    /// If either one can not be verified, a TLS connection will not be established. Default is false.
    /// </summary>
    public bool SkipVerification { get; set; } = false;
}