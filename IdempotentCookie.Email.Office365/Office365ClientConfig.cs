using System.ComponentModel.DataAnnotations;
using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Office365;

public class Office365ClientConfig : IEmailClientConfiguration
{
    public EmailProvider Provider => EmailProvider.Office365;

    [Required]
    [EmailAddress]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string Pwd { get; set; } = string.Empty;

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