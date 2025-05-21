using RestSharp;

namespace DdotM.EmailClient.Mailgun;

/// <inheritdoc />
public class MailgunClient(
    MailgunClientConfig mailgunClientConfig) : IMailgunClient
{
    private readonly MailgunClientConfig _mailgunClientConfig = mailgunClientConfig ?? throw new ArgumentNullException(nameof(mailgunClientConfig));

    /// <inheritdoc />
    public async Task<MailgunMessage> SendAsync(MailgunMessage msg)
    {
        // Mailgun API documentation: https://documentation.mailgun.com/en/latest/user_manual.html#sending-via-api
        var client = new RestClient("https://api.mailgun.net");

        var request = new RestRequest();
        request.AddHeader("Authorization", $"Basic {_mailgunClientConfig.ApiKey?.Base64Encode()}");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddParameter("domain", _mailgunClientConfig.SendingDomain, ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        // Send a message with custom connection settings
        request.AddParameter("o:require-tls", _mailgunClientConfig.RequireTls);
        request.AddParameter("o:skip-verification", _mailgunClientConfig.SkipVerification);

        request.AddParameter("from", $"{msg.From.Name} <{msg.From.Address}>");

        foreach (var toRecipient in msg.ToEmails)
        {
            request.AddParameter("to", $"{toRecipient.Name} <{toRecipient.Address}>");
        }

        if (!msg.ToEmails.Any())
        {
            request.AddParameter("to", $"{msg.From.Name} <{msg.From.Address}>");
        }

        foreach (var ccRecipient in msg.CcEmails)
        {
            request.AddParameter("cc", $"{ccRecipient.Name} <{ccRecipient.Address}>");
        }

        foreach (var bccRecipient in msg.BccEmails)
        {
            request.AddParameter("bcc", $"{bccRecipient.Name} <{bccRecipient.Address}>");
        }

        request.AddParameter("subject", msg.Subject);
        request.AddParameter("text", msg.TextBody);
        request.AddParameter("html", msg.HtmlBody);

        // This will disable link rewriting for this message
        request.AddParameter("o:tracking", msg.Tracking);
        // Set message delivery time - format "Fri, 14 Oct 2011 23:10:10 -0000"
        if (msg.DeliveryTime.HasValue)
        {
            request.AddParameter("o:deliverytime", msg.DeliveryTime.Value.ToString("ddd, dd MMM yyyy HH:mm:ss -0000"));
        }

        // Add tag(s)
        foreach (var tag in msg.Tags)
        {
            request.AddParameter("o:tag", tag);
        }

        request.Method = Method.Post;
        var response = await client.ExecuteAsync(request);

        msg.Response = response;

        return msg;
    }
}