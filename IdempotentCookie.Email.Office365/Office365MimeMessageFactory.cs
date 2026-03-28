using IdempotentCookie.Email;
using MimeKit;

namespace IdempotentCookie.Email.Office365;

internal sealed class Office365MimeMessageFactory : IOffice365MimeMessageFactory
{
    public MimeMessage Create(EmailMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var mimeMessage = new MimeMessage();

        AddAddress(mimeMessage.From, message.From);
        AddAddresses(mimeMessage.To, message.ToRecipients);
        AddAddresses(mimeMessage.Cc, message.CcRecipients);
        AddAddresses(mimeMessage.Bcc, message.BccRecipients);

        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = BuildBody(message);

        return mimeMessage;
    }

    private static MimeEntity BuildBody(EmailMessage message)
    {
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = string.IsNullOrWhiteSpace(message.HtmlBody) ? null : message.HtmlBody,
            TextBody = message.TextBody
        };

        foreach (var attachment in message.Attachments)
        {
            if (string.IsNullOrWhiteSpace(attachment.ContentType))
            {
                bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content);
                continue;
            }

            bodyBuilder.Attachments.Add(
                attachment.FileName,
                attachment.Content,
                ContentType.Parse(attachment.ContentType));
        }

        return bodyBuilder.ToMessageBody();
    }

    private static void AddAddresses(InternetAddressList destination, IEnumerable<EmailAddress> addresses)
    {
        foreach (var address in addresses)
        {
            AddAddress(destination, address);
        }
    }

    private static void AddAddress(InternetAddressList destination, EmailAddress address)
    {
        destination.Add(new MailboxAddress(address.Name, address.Address));
    }
}