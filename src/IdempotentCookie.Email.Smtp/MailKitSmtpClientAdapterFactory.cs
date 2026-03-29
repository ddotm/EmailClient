using MailKit.Net.Smtp;

namespace IdempotentCookie.Email.Smtp;

internal sealed class MailKitSmtpClientAdapterFactory : ISmtpClientAdapterFactory
{
    public ISmtpClientAdapter Create()
    {
        return new MailKitSmtpClientAdapter(new SmtpClient());
    }
}