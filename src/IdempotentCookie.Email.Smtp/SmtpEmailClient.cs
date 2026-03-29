using IdempotentCookie.Email;
using MailKit.Security;

namespace IdempotentCookie.Email.Smtp;

/// <summary>
/// Implements <see cref="IEmailClient"/> using a generic SMTP transport.
/// </summary>
internal sealed class SmtpEmailClient : IEmailClient
{
    private readonly SmtpClientConfig _config;
    private readonly ISmtpClientAdapterFactory _smtpClientFactory;
    private readonly ISmtpMimeMessageFactory _mimeMessageFactory;

    internal SmtpEmailClient(SmtpClientConfig config)
        : this(config, new MailKitSmtpClientAdapterFactory(), new SmtpMimeMessageFactory())
    {
    }

    internal SmtpEmailClient(
        SmtpClientConfig config,
        ISmtpClientAdapterFactory smtpClientFactory,
        ISmtpMimeMessageFactory mimeMessageFactory)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _config.Validate();
        _smtpClientFactory = smtpClientFactory ?? throw new ArgumentNullException(nameof(smtpClientFactory));
        _mimeMessageFactory = mimeMessageFactory ?? throw new ArgumentNullException(nameof(mimeMessageFactory));
    }

    /// <inheritdoc />
    public EmailProvider Provider => EmailProvider.Smtp;

    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var mimeMessage = _mimeMessageFactory.Create(message);
        await using var smtpClient = _smtpClientFactory.Create();
        var connected = false;

        try
        {
            await smtpClient.ConnectAsync(_config.Host, _config.Port, ToSecureSocketOptions(_config.Security), cancellationToken);
            connected = true;

            if (_config.RequiresAuthentication)
            {
                await smtpClient.AuthenticateAsync(_config.UserName, _config.Password, cancellationToken);
            }

            await smtpClient.SendAsync(mimeMessage, cancellationToken);
        }
        finally
        {
            if (connected)
            {
                await smtpClient.DisconnectAsync(true, cancellationToken);
            }
        }
    }

    private static SecureSocketOptions ToSecureSocketOptions(SmtpConnectionSecurity security)
    {
        return security switch
        {
            SmtpConnectionSecurity.None => SecureSocketOptions.None,
            SmtpConnectionSecurity.SslOnConnect => SecureSocketOptions.SslOnConnect,
            SmtpConnectionSecurity.StartTls => SecureSocketOptions.StartTls,
            SmtpConnectionSecurity.StartTlsWhenAvailable => SecureSocketOptions.StartTlsWhenAvailable,
            _ => SecureSocketOptions.Auto
        };
    }
}