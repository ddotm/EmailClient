namespace IdempotentCookie.Email;

/// <summary>
/// Represents an email message to be sent via a configured provider.
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Gets or sets the sender address for the message.
    /// </summary>
    public EmailAddress From { get; set; } = new();

    /// <summary>
    /// Gets or sets the primary recipients.
    /// </summary>
    public List<EmailAddress> ToRecipients { get; set; } = [];

    /// <summary>
    /// Gets or sets the carbon copy recipients.
    /// </summary>
    public List<EmailAddress> CcRecipients { get; set; } = [];

    /// <summary>
    /// Gets or sets the blind carbon copy recipients.
    /// </summary>
    public List<EmailAddress> BccRecipients { get; set; } = [];

    /// <summary>
    /// Gets or sets the message subject.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plain text message body.
    /// </summary>
    public string TextBody { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML message body.
    /// </summary>
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file attachments to include with the message.
    /// </summary>
    public List<EmailAttachment> Attachments { get; set; } = [];
}
