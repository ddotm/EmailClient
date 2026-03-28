using System.ComponentModel.DataAnnotations;
using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Office365;

/// <summary>
/// Configures the Office365 email provider.
/// </summary>
public class Office365ClientConfig : IEmailClientConfiguration
{
    /// <inheritdoc />
    public EmailProvider Provider => EmailProvider.Office365;

    /// <summary>
    /// Gets or sets the Office365 mailbox or user identifier.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Office365 password.
    /// </summary>
    [Required]
    public string Pwd { get; set; } = string.Empty;

    /// <inheritdoc />
    public void Validate()
    {
        var context = new ValidationContext(this, null, null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true);

        if (!isValid)
        {
            var messages = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
            throw new ValidationException($"{nameof(Office365ClientConfig)} validation failed: {messages}");
        }
    }
}