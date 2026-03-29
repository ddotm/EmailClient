using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Http;

namespace IdempotentCookie.Email.SendGrid;

internal sealed class SendGridClientAdapter(SendGridClientConfig config) : ISendGridClientAdapter
{
    private readonly ISendGridClient _client = new global::SendGrid.SendGridClient((config ?? throw new ArgumentNullException(nameof(config))).ApiKey);

    public async Task<SendGridResponse> SendEmailAsync(SendGridMessage message, CancellationToken cancellationToken)
    {
        var response = await _client.SendEmailAsync(message, cancellationToken);
        var body = await ReadBodyAsync(response.Body, cancellationToken);

        return new SendGridResponse(response.StatusCode, body);
    }

    private static async Task<string> ReadBodyAsync(HttpContent? body, CancellationToken cancellationToken)
    {
        if (body is null)
        {
            return string.Empty;
        }

        return await body.ReadAsStringAsync(cancellationToken);
    }
}