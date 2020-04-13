using Infrastructure.EmailManager;
using Infrastructure.EmailManager.EmailClients;
using System;
using System.Threading.Tasks;

namespace EmailManagerExe
{
    internal static class Program
    {
        private static EmailMessageConfig EmailMessageConfig { get; set; }
        private static EmailClientConfig EmailClientConfig { get; set; }

        private static async Task Main(string[] args)
        {
            EmailClientConfig = new EmailClientConfig();
            EmailMessageConfig = new EmailMessageConfig();

            CollectInput();
            //HardcodeInput();

            await TestOffice365Client();
            await TestMailgunClient();
        }

        private static void CollectInput()
        {
            Console.WriteLine($"Sender name: ");
            EmailMessageConfig.FromEmail.Name = Console.ReadLine();
            Console.WriteLine($"Sender email address: ");
            EmailMessageConfig.FromEmail.Address = Console.ReadLine();
            Console.WriteLine($"Sender password (for {EmailMessageConfig.FromEmail.Address})");
            EmailClientConfig.Id = EmailMessageConfig.FromEmail.Address;
            EmailClientConfig.Pwd = Console.ReadLine();

            Console.WriteLine($"Mailgun API key:");
            EmailClientConfig.MailgunApiKey = Console.ReadLine();
            Console.WriteLine($"Mailgun sending domain:");
            EmailClientConfig.MailgunSendingDomain = Console.ReadLine();

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
            EmailClientConfig.Id = EmailMessageConfig.FromEmail.Address;
            EmailClientConfig.Pwd = "";

            EmailClientConfig.MailgunApiKey = "";
            EmailClientConfig.MailgunSendingDomain = "";

            EmailMessageConfig.BccEmails.Add(new EmailRecipient());
            EmailMessageConfig.BccEmails[0].Name = "";
            EmailMessageConfig.BccEmails[0].Address = "";

            EmailMessageConfig.Subject = "Test message subject";
            EmailMessageConfig.TextBody = "Test message text";
            EmailMessageConfig.HtmlBody = $"<p>{EmailMessageConfig.TextBody}</p>";
        }

        private static async Task TestOffice365Client()
        {
            var emailClientConfig = new EmailClientConfig
                                    {
                                        EmailClientType = EmailClientType.Office365,
                                        Id = EmailClientConfig.Id,
                                        Pwd = EmailClientConfig.Pwd
                                    };
            await using var emailClient = new Office365EmailClient(emailClientConfig);
            await emailClient.SendAsync(EmailMessageConfig);
        }

        private static async Task TestMailgunClient()
        {
            var emailClientConfig = new EmailClientConfig
                                    {
                                        EmailClientType = EmailClientType.Mailgun,
                                        MailgunApiKey = EmailClientConfig.MailgunApiKey,
                                        MailgunSendingDomain = EmailClientConfig.MailgunSendingDomain
                                    };
            var emailClient = new MailgunEmailClient(emailClientConfig);
            await emailClient.SendAsync(EmailMessageConfig);
        }
    }
}
