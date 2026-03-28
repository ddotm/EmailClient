using MimeKit;

namespace IdempotentCookie.Email.Office365;

internal interface IOffice365MimeMessageFactory
{
    MimeMessage Create(EmailMessage message);
}