using RestSharp;

namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Mailgun email message
/// </summary>
public class MailgunMessage
{
    /// <summary>
    /// Email sender
    /// </summary>
    public Recipient From { get; } = new();

    /// <summary>
    /// List of the To email recipients
    /// </summary>
    public List<Recipient> ToEmails { get; } = new();

    /// <summary>
    /// List of the Cc email recipients
    /// </summary>
    public List<Recipient> CcEmails { get; } = new();

    /// <summary>
    /// List of the Bcc email recipients
    /// </summary>
    public List<Recipient> BccEmails { get; } = new();

    /// <summary>
    /// Email subject
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Email contents in text format
    /// </summary>
    public string TextBody { get; set; } = string.Empty;

    /// <summary>
    /// Email contents in HTML format
    /// </summary>
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>
    /// Message delivery time.
    /// Messages are not guaranteed to arrive at exactly the requested time.
    /// Messages can be scheduled for a maximum of 3 days in the future.
    /// </summary>
    public DateTime? DeliveryTime { get; set; }

    /// <summary>
    /// Mailgun message tags
    /// </summary>
    public List<string> Tags { get; } = new();

    /// <summary>
    /// Controls message tracking. Default is false.
    /// </summary>
    public bool Tracking { get; set; } = false;

    /// <summary>
    /// Once the message send is attempted, Response will contain the Mailgun HTTP response data
    /// </summary>
    public RestResponse Response { get; set; } = new();
}