using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager.EmailClients
{
    public class Office365EmailClient : IEmailClient, IAsyncDisposable
    {
        private readonly EmailClientConfig _emailClientConfig;
        private SmtpClient SmtpClient { get; set; }

        public Office365EmailClient(EmailClientConfig emailClientConfig)
        {
            _emailClientConfig = emailClientConfig;
        }

        public async Task SendAsync(EmailMessageConfig emailMessageConfig)
        {
            SmtpClient = await Office365SmtpClientAsync(_emailClientConfig.Id, _emailClientConfig.Pwd);

            var message = new MimeMessage();

            EmailComposer.AddFromAddress(message, emailMessageConfig.FromEmail.Name, emailMessageConfig.FromEmail.Address);
            foreach (var emailAddress in emailMessageConfig.ToEmails)
            {
                EmailComposer.AddToAddress(message, emailAddress.Name, emailAddress.Address);
            }

            foreach (var emailAddress in emailMessageConfig.CcEmails)
            {
                EmailComposer.AddCcAddress(message, emailAddress.Name, emailAddress.Address);
            }

            foreach (var emailAddress in emailMessageConfig.BccEmails)
            {
                EmailComposer.AddBccAddress(message, emailAddress.Name, emailAddress.Address);
            }

            message.Subject = emailMessageConfig.Subject;

            var bodyBuilder = new BodyBuilder
                              {
                                  HtmlBody = emailMessageConfig.HtmlBody,
                                  TextBody = emailMessageConfig.TextBody
                              };

            message.Body = bodyBuilder.ToMessageBody();

            await SmtpClient.SendAsync(message);
        }

        private static async Task<SmtpClient> Office365SmtpClientAsync(string user, string pwd)
        {
            var client = new SmtpClient();
            client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, pwd);

            return client;
        }

        private static async Task CloseAsync(IMailService smtpClient)
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
}
