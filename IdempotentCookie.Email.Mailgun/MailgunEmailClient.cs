using IdempotentCookie.Email;

namespace IdempotentCookie.Email.Mailgun;

/// <summary>
/// Implements <see cref="IEmailClient"/> for Mailgun by mapping the shared
/// <see cref="EmailMessage"/> contract onto the Mailgun-specific send path.
/// </summary>
internal sealed class MailgunEmailClient : IEmailClient
{
    private readonly IMailgunClient _client;

    internal MailgunEmailClient(MailgunClientConfig config)
        : this(new MailgunClient(config)) { }

    internal MailgunEmailClient(IMailgunClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <inheritdoc />
    public EmailProvider Provider => EmailProvider.Mailgun;

    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var mailgunMessage = MapToMailgunMessage(message);
        await _client.SendAsync(mailgunMessage);
    }

    private static MailgunMessage MapToMailgunMessage(EmailMessage message)
    {
        var mg = new MailgunMessage();

        mg.From.Name = message.From.Name;
        mg.From.Address = message.From.Address;

        mg.ToEmails.AddRange(message.ToRecipients.Select(r => new Recipient { Name = r.Name, Address = r.Address }));
        mg.CcEmails.AddRange(message.CcRecipients.Select(r => new Recipient { Name = r.Name, Address = r.Address }));
        mg.BccEmails.AddRange(message.BccRecipients.Select(r => new Recipient { Name = r.Name, Address = r.Address }));

        mg.Subject = message.Subject;
        mg.TextBody = message.TextBody;
        mg.HtmlBody = message.HtmlBody;

        // Attachments: deferred to Phase 4

        return mg;
    }
}
