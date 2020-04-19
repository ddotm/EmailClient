using System;
using System.Threading.Tasks;
using DdotM.EmailClient.Common;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DdotM.EmailClient.Office365
{
    public class Office365EmailClient : IOffice365EmailClient, IAsyncDisposable
    {
        private readonly Office365ClientConfig _office365ClientConfig;
        private SmtpClient SmtpClient { get; set; }

        public Office365EmailClient(Office365ClientConfig office365ClientConfig)
        {
            _office365ClientConfig = office365ClientConfig;
        }

        public async Task SendAsync(EmailMessageConfig emailMessageConfig)
        {
            SmtpClient = await Office365SmtpClientAsync(_office365ClientConfig.Id, _office365ClientConfig.Pwd);

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
