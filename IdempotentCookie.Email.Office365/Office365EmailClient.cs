using MailKit.Security;
using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Office365;

/// <summary>
/// Implements <see cref="IEmailClient"/> for Office365 SMTP delivery.
/// </summary>
public sealed class Office365EmailClient : IOffice365EmailClient
{
    private readonly Office365ClientConfig _office365ClientConfig;
    private readonly IOffice365SmtpClientAdapterFactory _smtpClientFactory;
    private readonly IOffice365MimeMessageFactory _mimeMessageFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Office365EmailClient"/> class.
    /// </summary>
    /// <param name="office365ClientConfig">Office365 configuration.</param>
    public Office365EmailClient(Office365ClientConfig office365ClientConfig)
        : this(office365ClientConfig, new Office365SmtpClientAdapterFactory(), new Office365MimeMessageFactory())
    {
    }

    internal Office365EmailClient(
        Office365ClientConfig office365ClientConfig,
        IOffice365SmtpClientAdapterFactory smtpClientFactory,
        IOffice365MimeMessageFactory mimeMessageFactory)
    {
        _office365ClientConfig = office365ClientConfig ?? throw new ArgumentNullException(nameof(office365ClientConfig));
        _office365ClientConfig.Validate();
        _smtpClientFactory = smtpClientFactory ?? throw new ArgumentNullException(nameof(smtpClientFactory));
        _mimeMessageFactory = mimeMessageFactory ?? throw new ArgumentNullException(nameof(mimeMessageFactory));
    }

    /// <inheritdoc />
    public EmailProvider Provider => EmailProvider.Office365;

    /// <inheritdoc />
    public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emailMessage);

        var message = _mimeMessageFactory.Create(emailMessage);
        await using var smtpClient = _smtpClientFactory.Create();
        var connected = false;

        try
        {
            await smtpClient.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls, cancellationToken);
            connected = true;
            await smtpClient.AuthenticateAsync(_office365ClientConfig.Id, _office365ClientConfig.Pwd, cancellationToken);
            await smtpClient.SendAsync(message, cancellationToken);
        }
        finally
        {
            if (connected)
            {
                await smtpClient.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}