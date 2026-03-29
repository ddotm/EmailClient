using MimeKit;

namespace IdempotentCookie.Email.Smtp;

internal interface ISmtpMimeMessageFactory
{
    MimeMessage Create(EmailMessage message);
}