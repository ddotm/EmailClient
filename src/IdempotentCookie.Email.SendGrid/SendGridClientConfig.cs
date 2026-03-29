using System.ComponentModel.DataAnnotations;
using IdempotentCookie.Email;

namespace IdempotentCookie.Email.SendGrid;

/// <summary>
/// Configures the SendGrid email provider.
/// </summary>
public class SendGridClientConfig : IEmailClientConfiguration
{
    /// <inheritdoc />
    public EmailProvider Provider => EmailProvider.SendGrid;

    /// <summary>
    /// Gets or sets the SendGrid API key.
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <inheritdoc />
    public void Validate()
    {
        var context = new ValidationContext(this, null, null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true);

        if (!isValid)
        {
            var messages = string.Join(Environment.NewLine, validationResults.Select(result => result.ErrorMessage));
            throw new ValidationException($"{nameof(SendGridClientConfig)} validation failed: {messages}");
        }
    }
}