using DdotM.EmailClient.Common;
using DdotM.EmailClient.Mailgun;
using DdotM.EmailClient.Office365;
using System;
using System.Threading.Tasks;

namespace EmailClientTester
{
    internal static class Program
    {
        private static EmailMessageConfig EmailMessageConfig { get; set; }
        private static Office365ClientConfig Office365ClientConfig { get; set; }

        private static async Task Main(string[] args)
        {
            Office365ClientConfig = new Office365ClientConfig();
            EmailMessageConfig = new EmailMessageConfig();

            // CollectInput();
            HardcodeInput();

            // await TestOffice365Client();
            await TestMailgunClient();
        }

        private static void CollectInput()
        {
            Console.WriteLine($"Sender name: ");
            EmailMessageConfig.FromEmail.Name = Console.ReadLine();
            Console.WriteLine($"Sender email address: ");
            EmailMessageConfig.FromEmail.Address = Console.ReadLine();
            Console.WriteLine($"Sender password (for {EmailMessageConfig.FromEmail.Address})");
            Office365ClientConfig.Id = EmailMessageConfig.FromEmail.Address;
            Office365ClientConfig.Pwd = Console.ReadLine();

            Console.WriteLine($"Mailgun API key:");
            Office365ClientConfig.MailgunApiKey = Console.ReadLine();
            Console.WriteLine($"Mailgun sending domain:");
            Office365ClientConfig.MailgunSendingDomain = Console.ReadLine();

            EmailMessageConfig.BccEmails.Add(new EmailRecipient());
            Console.WriteLine($"Name of recipient:");
            EmailMessageConfig.BccEmails[0].Name = Console.ReadLine();
            Console.WriteLine($"Recipient email address:");
            EmailMessageConfig.BccEmails[0].Address = Console.ReadLine();

            Console.WriteLine($"Email subject:");
            EmailMessageConfig.Subject = Console.ReadLine();
            Console.WriteLine($"Email text:");
            EmailMessageConfig.TextBody = Console.ReadLine();
            Console.Clear();
        }

        private static void HardcodeInput()
        {
            EmailMessageConfig.FromEmail.Name = "";
            EmailMessageConfig.FromEmail.Address = "";
            Office365ClientConfig.Id = EmailMessageConfig.FromEmail.Address;
            Office365ClientConfig.Pwd = "";

            Office365ClientConfig.MailgunApiKey = "";
            Office365ClientConfig.MailgunSendingDomain = "";

            EmailMessageConfig.BccEmails.Add(new EmailRecipient());
            EmailMessageConfig.BccEmails[0].Name = "";
            EmailMessageConfig.BccEmails[0].Address = "";

            EmailMessageConfig.Subject = "Test message subject";
            EmailMessageConfig.TextBody = "Test message text";
            EmailMessageConfig.HtmlBody = $"<html><body><p>{EmailMessageConfig.TextBody}</p></body></html>";
        }

        private static async Task TestOffice365Client()
        {
            var emailClientConfig = new Office365ClientConfig
                                    {
                                        EmailClientType = EmailClientType.Office365,
                                        Id = Office365ClientConfig.Id,
                                        Pwd = Office365ClientConfig.Pwd
                                    };
            await using var emailClient = new Office365EmailClient(emailClientConfig);
            await emailClient.SendAsync(EmailMessageConfig);
        }

        private static async Task TestMailgunClient()
        {
            var emailClientConfig = new MailgunClientConfig
                                    {
                                        MailgunApiKey = Office365ClientConfig.MailgunApiKey,
                                        MailgunSendingDomain = Office365ClientConfig.MailgunSendingDomain
                                    };
            var emailClient = new MailgunEmailClient(emailClientConfig);
            var response = await emailClient.SendAsync(EmailMessageConfig);
        }
    }
}
