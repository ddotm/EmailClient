namespace DdotM.EmailClient.Mailgun;

/// <inheritdoc />
public class MailgunRequestBuilder : IMailgunRequestBuilder
{
    private readonly MailgunClientConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailgunRequestBuilder"/> class.
    /// </summary>
    /// <param name="config">The Mailgun client configuration.</param>
    public MailgunRequestBuilder(MailgunClientConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <inheritdoc />
    public HttpContent Build(MailgunMessage msg)
    {
        var keyValues = BuildFieldDictionary(msg);
        
        return new FormUrlEncodedContent(keyValues);
    }

    /// <summary>
    /// Builds the key-value pairs for form fields, including message and config fields.
    /// </summary>
    /// <param name="msg">The message to convert to key-value pairs.</param>
    /// <returns>A list of form fields for the request.</returns>
    protected virtual List<KeyValuePair<string, string>> BuildFieldDictionary(MailgunMessage msg)
    {
        var keyValues = new List<KeyValuePair<string, string>>
        {
            new("from", msg.From.ToFullAddress()),
            new("subject", msg.Subject),
            new("text", msg.TextBody),
            new("html", string.IsNullOrWhiteSpace(msg.HtmlBody) ? msg.TextBody : msg.HtmlBody),
            new("o:require-tls", _config.RequireTls ? "yes" : "no"),
            new("o:skip-verification", _config.SkipVerification ? "yes" : "no"),
            new("o:tracking", msg.Tracking ? "yes" : "no")
        };

        keyValues.AddRange(msg.ToEmails.Select(to => new KeyValuePair<string, string>("to", to.ToFullAddress())));
        keyValues.AddRange(msg.CcEmails.Select(cc => new KeyValuePair<string, string>("cc", cc.ToFullAddress())));
        keyValues.AddRange(msg.BccEmails.Select(bcc => new KeyValuePair<string, string>("bcc", bcc.ToFullAddress())));
        keyValues.AddRange(msg.Tags.Select(tag => new KeyValuePair<string, string>("o:tag", tag)));

        // Only add delivery time if explicitly set
        if (msg.DeliveryTime.HasValue)
        {
            keyValues.Add(new("o:deliverytime", msg.DeliveryTime.Value.ToString("ddd, dd MMM yyyy HH:mm:ss -0000")));
        }

        // Add future custom fields here...

        return keyValues;
    }
}
