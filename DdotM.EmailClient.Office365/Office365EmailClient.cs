using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DdotM.EmailClient.Office365;

public class Office365EmailClient(Office365ClientConfig office365ClientConfig) : IOffice365EmailClient, IAsyncDisposable
{
    private readonly Office365ClientConfig _office365ClientConfig = office365ClientConfig ?? throw new ArgumentNullException(nameof(office365ClientConfig));
    
    private SmtpClient SmtpClient { get; set; }

    public async Task SendAsync(EmailMessage emailMessage)
    {
        SmtpClient = await Office365SmtpClientAsync(_office365ClientConfig.Id, _office365ClientConfig.Pwd);

        var message = new MimeMessage();

        EmailComposer.AddFromAddress(message, emailMessage.FromEmail.Name, emailMessage.FromEmail.Address);
        foreach (var emailAddress in emailMessage.ToEmails)
        {
            EmailComposer.AddToAddress(message, emailAddress.Name, emailAddress.Address);
        }

        foreach (var emailAddress in emailMessage.CcEmails)
        {
            EmailComposer.AddCcAddress(message, emailAddress.Name, emailAddress.Address);
        }

        foreach (var emailAddress in emailMessage.BccEmails)
        {
            EmailComposer.AddBccAddress(message, emailAddress.Name, emailAddress.Address);
        }

        message.Subject = emailMessage.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = emailMessage.HtmlBody,
            TextBody = emailMessage.TextBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        await SmtpClient.SendAsync(message);
    }

    private static async Task<SmtpClient> Office365SmtpClientAsync(string user, string pwd)
    {
        var client = new SmtpClient();
        await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(user, pwd);

        return client;
    }

    private static async Task CloseAsync(IMailService? smtpClient)
    {
        if (smtpClient != null)
        {
            await smtpClient.DisconnectAsync(true);
            smtpClient.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await CloseAsync(SmtpClient);
    }
}