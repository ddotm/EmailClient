using System.ComponentModel.DataAnnotations;
using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Smtp;

/// <summary>
/// Configures the generic SMTP email provider.
/// </summary>
public class SmtpClientConfig : IEmailClientConfiguration
{
    /// <inheritdoc />
    public EmailProvider Provider => EmailProvider.Smtp;

    /// <summary>
    /// Gets or sets the SMTP server host name.
    /// </summary>
    [Required]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SMTP server port.
    /// </summary>
    [Range(1, 65535)]
    public int Port { get; set; } = 587;

    /// <summary>
    /// Gets or sets the transport security mode.
    /// </summary>
    public SmtpConnectionSecurity Security { get; set; } = SmtpConnectionSecurity.StartTls;

    /// <summary>
    /// Gets or sets the SMTP user name.
    /// Leave empty to skip authentication.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SMTP password.
    /// Leave empty to skip authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    internal bool RequiresAuthentication =>
        !string.IsNullOrWhiteSpace(UserName) ||
        !string.IsNullOrWhiteSpace(Password);

    /// <inheritdoc />
    public void Validate()
    {
        var context = new ValidationContext(this, null, null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true);

        if (!isValid)
        {
            var messages = string.Join(Environment.NewLine, validationResults.Select(result => result.ErrorMessage));
            throw new ValidationException($"{nameof(SmtpClientConfig)} validation failed: {messages}");
        }

        var hasUserName = !string.IsNullOrWhiteSpace(UserName);
        var hasPassword = !string.IsNullOrWhiteSpace(Password);

        if (hasUserName != hasPassword)
        {
            throw new ValidationException($"{nameof(SmtpClientConfig)} validation failed: UserName and Password must both be provided when SMTP authentication is enabled.");
        }
    }
}