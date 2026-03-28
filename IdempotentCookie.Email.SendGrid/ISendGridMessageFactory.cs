using SendGrid.Helpers.Mail;

namespace IdempotentCookie.Email.SendGrid;

internal interface ISendGridMessageFactory
{
    SendGridMessage Create(EmailMessage message);
}