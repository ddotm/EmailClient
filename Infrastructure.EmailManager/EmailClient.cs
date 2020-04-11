using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

namespace Infrastructure.EmailManager
{
    public static class EmailClient
    {
        public static async Task SendAsync(EmailClientConfig emailClientConfig)
        {
            var message = new MimeMessage();

            EmailComposer.AddFromAddress(message, emailClientConfig.FromEmail.Name, emailClientConfig.FromEmail.Address);
            foreach (var emailAddress in emailClientConfig.ToEmails)
            {
                EmailComposer.AddToAddress(message, emailAddress.Name, emailAddress.Address);
            }

            foreach (var emailAddress in emailClientConfig.CcEmails)
            {
                EmailComposer.AddCcAddress(message, emailAddress.Name, emailAddress.Address);
            }

            foreach (var emailAddress in emailClientConfig.BccEmails)
            {
                EmailComposer.AddBccAddress(message, emailAddress.Name, emailAddress.Address);
            }

            message.Subject = emailClientConfig.Subject;

            var bodyBuilder = new BodyBuilder
                              {
                                  HtmlBody = emailClientConfig.HtmlBody,
                                  TextBody = emailClientConfig.TextBody
                              };

            message.Body = bodyBuilder.ToMessageBody();

            var smtpClient = await Office365SmtpClientAsync(emailClientConfig.Id, emailClientConfig.Pwd);

            await smtpClient.SendAsync(message);

            await CloseAsync(smtpClient);
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
            await smtpClient.DisconnectAsync(true);
            smtpClient.Dispose();
        }
    }
}
