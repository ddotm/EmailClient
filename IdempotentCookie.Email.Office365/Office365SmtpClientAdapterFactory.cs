using MailKit.Net.Smtp;

namespace IdempotentCookie.Email.Office365;

internal sealed class Office365SmtpClientAdapterFactory : IOffice365SmtpClientAdapterFactory
{
    public IOffice365SmtpClientAdapter Create()
    {
        return new Office365SmtpClientAdapter(new SmtpClient());
    }
}