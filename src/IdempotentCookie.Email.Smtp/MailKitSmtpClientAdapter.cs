using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace IdempotentCookie.Email.Smtp;

internal sealed class MailKitSmtpClientAdapter(SmtpClient client) : ISmtpClientAdapter
{
    private readonly SmtpClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public Task ConnectAsync(string host, int port, SecureSocketOptions security, CancellationToken cancellationToken)
    {
        return _client.ConnectAsync(host, port, security, cancellationToken);
    }

    public Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
    {
        return _client.AuthenticateAsync(userName, password, cancellationToken);
    }

    public Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        return _client.SendAsync(message, cancellationToken);
    }

    public Task DisconnectAsync(bool quit, CancellationToken cancellationToken)
    {
        return _client.DisconnectAsync(quit, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}