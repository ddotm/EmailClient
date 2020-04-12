using Infrastructure.EmailManager;
using Infrastructure.EmailManager.EmailClients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailManagerExe
{
    internal static class Program
    {
        private static EmailMessageConfig EmailMessageConfig { get; set; }
        private static string SenderName { get; set; }
        private static string SenderEmail { get; set; }
        private static string SenderPwd { get; set; }
        private static string ApiKey { get; set; }
        private static string RecipientName { get; set; }
        private static string RecipientEmail { get; set; }
        private static string Subject { get; set; }
        private static string BodyText { get; set; }

        private static async Task Main(string[] args)
        {
            CollectInput();
            // HardcodeInput();
            PopulateMessage();

            await TestOffice365Client();
            await TestMailgunClient();
        }

        private static void CollectInput()
        {
            Console.WriteLine($"Sender name: ");
            SenderName = Console.ReadLine();
            Console.WriteLine($"Sender email address: ");
            SenderEmail = Console.ReadLine();
            Console.WriteLine($"Sender password (for {SenderEmail})");
            SenderPwd = Console.ReadLine();

            Console.WriteLine($"Mailgun API key:");
            ApiKey = Console.ReadLine();

            Console.WriteLine($"Name of recipient:");
            RecipientName = Console.ReadLine();
            Console.WriteLine($"Recipient email address:");
            RecipientEmail = Console.ReadLine();

            Console.WriteLine($"Email subject:");
            Subject = Console.ReadLine();
            Console.WriteLine($"Email text:");
            BodyText = Console.ReadLine();
            Console.Clear();
        }

        private static void HardcodeInput()
        {
            SenderName = "";
            SenderEmail = "";
            SenderPwd = "";

            ApiKey = "";

            RecipientName = "";
            RecipientEmail = "";

            Subject = "Test message subject";
            BodyText = "Test message text";
        }

        private static void PopulateMessage()
        {
            EmailMessageConfig = new EmailMessageConfig
                                 {
                                     FromEmail = new EmailRecipient
                                                 {
                                                     Name = SenderName,
                                                     Address = SenderEmail
                                                 },
                                     BccEmails = new List<EmailRecipient>
                                                 {
                                                     new EmailRecipient
                                                     {
                                                         Name = RecipientName,
                                                         Address = RecipientEmail
                                                     }
                                                 },
                                     Subject = Subject,
                                     TextBody = BodyText,
                                     HtmlBody = $"<p>{BodyText}</p>"
                                 };
        }

        private static async Task TestOffice365Client()
        {
            var emailClientConfig = new EmailClientConfig
                                    {
                                        EmailClientType = EmailClientType.Office365,
                                        Id = SenderEmail,
                                        Pwd = SenderPwd
                                    };
            await using var emailClient = new Office365EmailClient(emailClientConfig);
            await emailClient.SendAsync(EmailMessageConfig);
        }

        private static async Task TestMailgunClient()
        {
            var emailClientConfig = new EmailClientConfig
                                    {
                                        EmailClientType = EmailClientType.Mailgun,
                                        ApiKey = ApiKey
                                    };
            var emailClient = new MailgunEmailClient(emailClientConfig);
            await emailClient.SendAsync(EmailMessageConfig);
        }
    }
}
