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
        var endpoint = GetEndpoint();
        var content = BuildFormContent(msg);
        using var httpClient = CreateHttpClient();

        var response = await httpClient.PostAsync(endpoint, content);
        msg.Response = response;

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Mailgun API request failed with status code {response.StatusCode}: {error}");
        }

        return msg;
    }

    private string GetEndpoint()
    {
        return $"https://api.mailgun.net/v3/{_mailgunClientConfig.SendingDomain}/messages";
    }

    private FormUrlEncodedContent BuildFormContent(MailgunMessage msg)
    {
        var keyValues = new List<KeyValuePair<string, string>>
        {
            new("from", msg.From.ToFullAddress()),
            new("subject", msg.Subject),
            new("text", msg.TextBody),
            new("html", string.IsNullOrWhiteSpace(msg.HtmlBody) ? msg.TextBody : msg.HtmlBody),
            new("o:require-tls", _mailgunClientConfig.RequireTls ? "yes" : "no"),
            new("o:skip-verification", _mailgunClientConfig.SkipVerification ? "yes" : "no"),
            new("o:tracking", msg.Tracking ? "yes" : "no")
        };

        keyValues.AddRange(msg.ToEmails.Select(to => new KeyValuePair<string, string>("to", to.ToFullAddress())));
        keyValues.AddRange(msg.CcEmails.Select(cc => new KeyValuePair<string, string>("cc", cc.ToFullAddress())));
        keyValues.AddRange(msg.BccEmails.Select(bcc => new KeyValuePair<string, string>("bcc", bcc.ToFullAddress())));
        keyValues.AddRange(msg.Tags.Select(tag => new KeyValuePair<string, string>("o:tag", tag)));

        if (msg.DeliveryTime.HasValue)
        {
            keyValues.Add(new("o:deliverytime",
                msg.DeliveryTime.Value.ToString("ddd, dd MMM yyyy HH:mm:ss -0000")));
        }

        return new FormUrlEncodedContent(keyValues);
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        var credentials = $"{_mailgunClientConfig.ApiUser}:{_mailgunClientConfig.ApiKey}";
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        return httpClient;
    }
}