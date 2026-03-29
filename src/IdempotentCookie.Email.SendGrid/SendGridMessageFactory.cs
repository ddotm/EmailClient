using IdempotentCookie.Email;
using SendGrid.Helpers.Mail;
using SendGridEmailAddress = SendGrid.Helpers.Mail.EmailAddress;

namespace IdempotentCookie.Email.SendGrid;

internal sealed class SendGridMessageFactory : ISendGridMessageFactory
{
    public SendGridMessage Create(EmailMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var sendGridMessage = new SendGridMessage
        {
            Subject = message.Subject,
            PlainTextContent = message.TextBody,
            HtmlContent = message.HtmlBody
        };

        sendGridMessage.SetFrom(ToSendGridEmailAddress(message.From));

        foreach (var address in message.ToRecipients)
        {
            sendGridMessage.AddTo(ToSendGridEmailAddress(address));
        }

        foreach (var address in message.CcRecipients)
        {
            sendGridMessage.AddCc(ToSendGridEmailAddress(address));
        }

        foreach (var address in message.BccRecipients)
        {
            sendGridMessage.AddBcc(ToSendGridEmailAddress(address));
        }

        foreach (var attachment in message.Attachments)
        {
            sendGridMessage.AddAttachment(
                attachment.FileName,
                Convert.ToBase64String(attachment.Content),
                attachment.ContentType);
        }

        return sendGridMessage;
    }

    private static SendGridEmailAddress ToSendGridEmailAddress(EmailAddress address)
    {
        return new SendGridEmailAddress(address.Address, address.Name);
    }
}