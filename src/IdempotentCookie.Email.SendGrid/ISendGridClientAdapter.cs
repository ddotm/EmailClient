using SendGrid.Helpers.Mail;

namespace IdempotentCookie.Email.SendGrid;

internal interface ISendGridClientAdapter
{
    Task<SendGridResponse> SendEmailAsync(SendGridMessage message, CancellationToken cancellationToken);
}