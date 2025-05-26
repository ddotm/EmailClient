using System.Net.Http.Headers;
using System.Text;

namespace DdotM.EmailClient.Mailgun;

/// <summary>
/// Implementation of <see cref="IMailgunClient"/> for sending emails via Mailgun.
/// </summary>
public class MailgunClient : IMailgunClient
{
    private readonly MailgunClientConfig _mailgunClientConfig;
    private readonly IHttpClientAdapter _httpClientAdapter;
    private readonly IMailgunRequestBuilder _requestBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailgunClient"/> class with the specified configuration.
    /// Validates the configuration before use.
    /// </summary>
    /// <param name="mailgunClientConfig">
    /// The <see cref="MailgunClientConfig"/> containing Mailgun API key, sending domain, and other settings.
    /// </param>
    /// <param name="httpClientAdapter">The <see cref="IHttpClientAdapter"/> to be used for HTTP requests.</param>
    /// <param name="requestBuilder">The <see cref="IMailgunRequestBuilder"/> responsible for building HTTP content from the message and config.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mailgunClientConfig"/>, <paramref name="httpClientAdapter"/>, or <paramref name="requestBuilder"/> is null.</exception>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">
    /// Thrown if the configuration is invalid.
    /// </exception>
    public MailgunClient(
        MailgunClientConfig mailgunClientConfig,
        IHttpClientAdapter httpClientAdapter,
        IMailgunRequestBuilder requestBuilder)
    {
        _mailgunClientConfig = mailgunClientConfig ?? throw new ArgumentNullException(nameof(mailgunClientConfig));
        _mailgunClientConfig.Validate();

        _httpClientAdapter = httpClientAdapter ?? throw new ArgumentNullException(nameof(httpClientAdapter));
        _requestBuilder = requestBuilder ?? throw new ArgumentNullException(nameof(requestBuilder));
    }

    /// <inheritdoc />
    public async Task<MailgunMessage> SendAsync(MailgunMessage msg)
    {
        var endpoint = _mailgunClientConfig.MailgunApiEndpoint;
        var content = _requestBuilder.Build(msg);

        // Add headers
        var credentials = $"{_mailgunClientConfig.ApiUser}:{_mailgunClientConfig.ApiKey}";
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        _httpClientAdapter.AddHeader("Authorization", new AuthenticationHeaderValue("Basic", authToken).ToString());

        var response = await _httpClientAdapter.PostAsync(endpoint, content);
        msg.Response = response;

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Mailgun API request failed with status code {response.StatusCode}: {error}");
        }

        return msg;
    }
}