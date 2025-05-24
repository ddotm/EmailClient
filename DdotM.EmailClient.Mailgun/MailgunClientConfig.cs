using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Configures Mailgun client with API key, sending domain, and TLS settings.
/// </summary>
public class MailgunClientConfig
{
    /// <summary>
    /// Mailgun API user.
    /// </summary>
    public readonly string ApiUser = "api";

    /// <summary>
    /// Mailgun Sending key.
    /// Set up at https://app.mailgun.com/mg/sending/[SENDING_DOMAIN]/settings?tab=keys
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Mailgun Sending domain
    /// Set up at https://app.mailgun.com/mg/sending/domains
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

    /// <summary>
    /// Validates all required fields and sending domain format.
    /// Throws ValidationException if invalid.
    /// </summary>
    public void Validate()
    {
        // Validate
        var context = new ValidationContext(this, null, null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true);

        if (!isValid)
        {
            // Compose all errors into one exception message, or throw the first
            var messages = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
            throw new ValidationException($"{nameof(MailgunClientConfig)} validation failed: {messages}");
        }

        // Validate SendingDomain with a regex
        // Accepts domains like 'mg.example.com' or 'example.co.uk'
        if (!IsValidDomain(SendingDomain))
        {
            throw new ValidationException($"'{SendingDomain}' is not a valid sending domain.");
        }
    }

    private static bool IsValidDomain(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            return false;
        }

        // Simple and readable domain regex (not fully RFC strict)
        var domainPattern = @"^([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$";
        
        return Regex.IsMatch(domain, domainPattern);
    }
}