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
        var client = new RestClient();

        var endpoint = $"https://api.mailgun.net/v3/{_mailgunClientConfig.SendingDomain}/messages";

        var credentials = $"{_mailgunClientConfig.ApiUser}:{_mailgunClientConfig.ApiKey}";
        var authToken = credentials.Base64Encode();

        var request = new RestRequest(endpoint, Method.Post);
        
        request.AddHeader("Authorization", $"Basic {authToken}");
        
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddParameter("from", $"{msg.From.ToFullAddress()}");

        foreach (var toRecipient in msg.ToEmails)
        {
            request.AddParameter("to", $"{toRecipient.ToFullAddress()}");
        }

        if (!msg.ToEmails.Any())
        {
            request.AddParameter("to", $"{msg.From.ToFullAddress()}");
        }

        foreach (var ccRecipient in msg.CcEmails)
        {
            request.AddParameter("cc", $"{ccRecipient.ToFullAddress()}");
        }

        foreach (var bccRecipient in msg.BccEmails)
        {
            request.AddParameter("bcc", $"{bccRecipient.ToFullAddress()}");
        }

        request.AddParameter("subject", msg.Subject);
        request.AddParameter("text", msg.TextBody);
        request.AddParameter("html", msg.HtmlBody);

        // Send a message with custom connection settings
        request.AddParameter("o:require-tls", _mailgunClientConfig.RequireTls ? "yes" : "no");
        request.AddParameter("o:skip-verification", _mailgunClientConfig.SkipVerification ? "yes" : "no");
        
        // This will disable link rewriting for this message
        request.AddParameter("o:tracking", msg.Tracking ? "yes" : "no");
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

        var response = await client.ExecuteAsync(request);

        msg.Response = response;

        return msg;
    }
}