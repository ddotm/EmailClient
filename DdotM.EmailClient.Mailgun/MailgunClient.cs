using System.Net.Http.Headers;
using System.Text;

namespace DdotM.EmailClient.Mailgun;

public class MailgunClient : IMailgunClient
{
    private readonly MailgunClientConfig _mailgunClientConfig;

    public MailgunClient(MailgunClientConfig mailgunClientConfig)
    {
        _mailgunClientConfig = mailgunClientConfig ?? throw new ArgumentNullException(nameof(mailgunClientConfig));
    }

    public async Task<MailgunMessage> SendAsync(MailgunMessage msg)
    {
        var endpoint = $"https://api.mailgun.net/v3/{_mailgunClientConfig.SendingDomain}/messages";
        // Auth
        var credentials = $"{_mailgunClientConfig.ApiUser}:{_mailgunClientConfig.ApiKey}";
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

        // Prepare form data
        var keyValues = new List<KeyValuePair<string, string>>
        {
            new("from", msg.From.ToFullAddress()),
            new("subject", msg.Subject),
            new("text", msg.TextBody),
            new("html", msg.HtmlBody),
            new("o:require-tls", _mailgunClientConfig.RequireTls ? "yes" : "no"),
            new("o:skip-verification", _mailgunClientConfig.SkipVerification ? "yes" : "no"),
            new("o:tracking", msg.Tracking ? "yes" : "no")
        };

        // Tags
        keyValues.AddRange(msg.Tags.Select(tag => new KeyValuePair<string, string>("o:tag", tag)));

        // Recipients
        keyValues.AddRange(msg.ToEmails.Select(to => new KeyValuePair<string, string>("to", to.ToFullAddress())));
        keyValues.AddRange(msg.CcEmails.Select(cc => new KeyValuePair<string, string>("cc", cc.ToFullAddress())));
        keyValues.AddRange(msg.BccEmails.Select(bcc => new KeyValuePair<string, string>("bcc", bcc.ToFullAddress())));

        // Delivery time
        if (msg.DeliveryTime.HasValue)
        {
            keyValues.Add(new("o:deliverytime", msg.DeliveryTime.Value.ToString("ddd, dd MMM yyyy HH:mm:ss -0000")));
        }

        // Form content
        using var content = new FormUrlEncodedContent(keyValues);
        
        var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        // POST
        var response = await httpClient.PostAsync(endpoint, content);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Mailgun API request failed with status code {response.StatusCode}");
        }
        
        msg.Response = response;

        return msg;
    }
}