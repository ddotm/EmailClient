using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Mailgun;

/// <inheritdoc />
internal class MailgunRequestBuilder : IMailgunRequestBuilder
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
    public HttpContent Build(EmailMessage msg)
    {
        var keyValues = BuildFieldDictionary(msg);

        if (msg.Attachments.Count == 0)
        {
            return new FormUrlEncodedContent(keyValues);
        }

        var content = new MultipartFormDataContent();

        foreach (var keyValue in keyValues)
        {
            content.Add(new StringContent(keyValue.Value), keyValue.Key);
        }

        foreach (var attachment in msg.Attachments)
        {
            var attachmentContent = new ByteArrayContent(attachment.Content);

            if (!string.IsNullOrWhiteSpace(attachment.ContentType))
            {
                attachmentContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(attachment.ContentType);
            }

            content.Add(attachmentContent, "attachment", attachment.FileName);
        }

        return content;
    }

    /// <summary>
    /// Builds the key-value pairs for form fields, including message and config fields.
    /// </summary>
    /// <param name="msg">The message to convert to key-value pairs.</param>
    /// <returns>A list of form fields for the request.</returns>
    protected virtual List<KeyValuePair<string, string>> BuildFieldDictionary(EmailMessage msg)
    {
        var keyValues = new List<KeyValuePair<string, string>>
        {
            new("from", ToFullAddress(msg.From)),
            new("subject", msg.Subject),
            new("text", msg.TextBody),
            new("html", string.IsNullOrWhiteSpace(msg.HtmlBody) ? msg.TextBody : msg.HtmlBody),
            new("o:require-tls", _config.RequireTls ? "yes" : "no"),
            new("o:skip-verification", _config.SkipVerification ? "yes" : "no")
        };

        keyValues.AddRange(msg.ToRecipients.Select(to => new KeyValuePair<string, string>("to", ToFullAddress(to))));
        keyValues.AddRange(msg.CcRecipients.Select(cc => new KeyValuePair<string, string>("cc", ToFullAddress(cc))));
        keyValues.AddRange(msg.BccRecipients.Select(bcc => new KeyValuePair<string, string>("bcc", ToFullAddress(bcc))));

        return keyValues;
    }

    private static string ToFullAddress(EmailAddress address)
    {
        return string.IsNullOrWhiteSpace(address.Name)
            ? address.Address
            : $"{address.Name} <{address.Address}>";
    }
}
