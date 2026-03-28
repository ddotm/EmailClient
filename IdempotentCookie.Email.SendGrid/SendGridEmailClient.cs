using System.Net;
using IdempotentCookie.Email;

namespace IdempotentCookie.Email.SendGrid;

/// <summary>
/// Implements <see cref="IEmailClient"/> using the SendGrid API.
/// </summary>
internal sealed class SendGridEmailClient : IEmailClient
{
    private readonly SendGridClientConfig _config;
    private readonly ISendGridClientAdapterFactory _clientAdapterFactory;
    private readonly ISendGridMessageFactory _messageFactory;

    internal SendGridEmailClient(SendGridClientConfig config)
        : this(config, new SendGridClientAdapterFactory(), new SendGridMessageFactory())
    {
    }

    internal SendGridEmailClient(
        SendGridClientConfig config,
        ISendGridClientAdapterFactory clientAdapterFactory,
        ISendGridMessageFactory messageFactory)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _config.Validate();
        _clientAdapterFactory = clientAdapterFactory ?? throw new ArgumentNullException(nameof(clientAdapterFactory));
        _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
    }

    /// <inheritdoc />
    public EmailProvider Provider => EmailProvider.SendGrid;

    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var client = _clientAdapterFactory.Create(_config);
        var payload = _messageFactory.Create(message);
        var response = await client.SendEmailAsync(payload, cancellationToken);

        if (!IsSuccessStatusCode(response.StatusCode))
        {
            throw new HttpRequestException(
                $"SendGrid API request failed with status code {response.StatusCode}: {response.Body}",
                null,
                response.StatusCode);
        }
    }

    private static bool IsSuccessStatusCode(HttpStatusCode statusCode)
    {
        var statusCodeValue = (int)statusCode;
        return statusCodeValue >= 200 && statusCodeValue < 300;
    }
}