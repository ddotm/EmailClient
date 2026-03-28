using MailKit.Security;
using MimeKit;

namespace IdempotentCookie.Email.Office365;

internal interface IOffice365SmtpClientAdapter : IAsyncDisposable
{
    Task ConnectAsync(string host, int port, SecureSocketOptions security, CancellationToken cancellationToken);

    Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken);

    Task SendAsync(MimeMessage message, CancellationToken cancellationToken);

    Task DisconnectAsync(bool quit, CancellationToken cancellationToken);
}