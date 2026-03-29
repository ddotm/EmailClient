using MailKit.Security;
using MimeKit;

namespace IdempotentCookie.Email.Smtp;

internal interface ISmtpClientAdapter : IAsyncDisposable
{
    Task ConnectAsync(string host, int port, SecureSocketOptions security, CancellationToken cancellationToken);

    Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken);

    Task SendAsync(MimeMessage message, CancellationToken cancellationToken);

    Task DisconnectAsync(bool quit, CancellationToken cancellationToken);
}