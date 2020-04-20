using DdotM.EmailClient.Common;
using DdotM.EmailClient.Mailgun;
using DdotM.EmailClient.Office365;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace EmailClientTester
{
    internal static class Program
    {
        private static EmailMessageConfig EmailMessageConfig { get; set; }
        private static MailgunMessage MailgunMessage { get; set; }
        private static Office365ClientConfig Office365ClientConfig { get; set; }
        private static MailgunClientConfig MailgunClientConfig { get; set; }

        private static async Task Main(string[] args)
        {
            Office365ClientConfig = new Office365ClientConfig();
            MailgunClientConfig = new MailgunClientConfig();

            MailgunMessage = new MailgunMessage();

            // CollectInputForOffice365Email();
            // HardcodeInputForOffice365();

            // CollectInputForMailgunEmail();
            HardcodeInputForMailgun();

            // await TestOffice365Client();
            var response = await TestMailgunClient();
            Console.WriteLine($"Email sent with response code {response.StatusCode}");
        }

        private static void CollectInputForOffice365Email()
        {
            Console.WriteLine($"Sender name: ");
            EmailMessageConfig.FromEmail.Name = Console.ReadLine();
            Console.WriteLine($"Sender email address: ");
            EmailMessageConfig.FromEmail.Address = Console.ReadLine();
            Console.WriteLine($"Sender password (for {EmailMessageConfig.FromEmail.Address})");
            Office365ClientConfig.Id = EmailMessageConfig.FromEmail.Address;
            Office365ClientConfig.Pwd = Console.ReadLine();

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

        private static void CollectInputForMailgunEmail()
        {
            Console.WriteLine($"Sender name: ");
            MailgunMessage.FromEmail.Name = Console.ReadLine();
            Console.WriteLine($"Sender email address: ");
            MailgunMessage.FromEmail.Address = Console.ReadLine();

            Console.WriteLine($"Mailgun API key:");
            MailgunClientConfig.ApiKey = Console.ReadLine();
            Console.WriteLine($"Mailgun sending domain:");
            MailgunClientConfig.SendingDomain = Console.ReadLine();

            MailgunMessage.BccEmails.Add(new EmailRecipient());
            Console.WriteLine($"Name of recipient:");
            MailgunMessage.BccEmails[0].Name = Console.ReadLine();
            Console.WriteLine($"Recipient email address:");
            MailgunMessage.BccEmails[0].Address = Console.ReadLine();

            Console.WriteLine($"Email subject:");
            MailgunMessage.Subject = Console.ReadLine();
            Console.WriteLine($"Email text:");
            MailgunMessage.TextBody = Console.ReadLine();
            Console.Clear();
        }

        private static void HardcodeInputForOffice365()
        {
            EmailMessageConfig.FromEmail.Name = "";
            EmailMessageConfig.FromEmail.Address = "";

            Office365ClientConfig.Id = EmailMessageConfig.FromEmail.Address;
            Office365ClientConfig.Pwd = "";

            EmailMessageConfig.BccEmails.Add(new EmailRecipient
                                             {
                                                 Name = "",
                                                 Address = ""
                                             });

            EmailMessageConfig.Subject = "Test message subject";
            EmailMessageConfig.TextBody = "Test message text";
            EmailMessageConfig.HtmlBody = $"<html><body><p>{EmailMessageConfig.TextBody}</p></body></html>";
        }

        private static void HardcodeInputForMailgun()
        {
            MailgunClientConfig.ApiKey = "";
            MailgunClientConfig.SendingDomain = "";
            MailgunClientConfig.RequireTls = true;
            MailgunClientConfig.SkipVerification = false;

            MailgunMessage.FromEmail.Name = "";
            MailgunMessage.FromEmail.Address = "";

            MailgunMessage.BccEmails.Add(new EmailRecipient
                                         {
                                             Name = "",
                                             Address = ""
                                         });

            MailgunMessage.Subject = "Test message subject";
            MailgunMessage.TextBody = "Test message text";
            MailgunMessage.HtmlBody = $"<html><body><p>{MailgunMessage.TextBody}</p></body></html>";

            MailgunMessage.Tags.Add("registration");
        }

        private static async Task TestOffice365Client()
        {
            var emailClientConfig = new Office365ClientConfig
                                    {
                                        Id = Office365ClientConfig.Id,
                                        Pwd = Office365ClientConfig.Pwd
                                    };
            await using var emailClient = new Office365EmailClient(emailClientConfig);
            await emailClient.SendAsync(EmailMessageConfig);
        }

        private static async Task<IRestResponse> TestMailgunClient()
        {
            var mailgunClientConfig = new MailgunClientConfig
                                      {
                                          ApiKey = MailgunClientConfig.ApiKey,
                                          SendingDomain = MailgunClientConfig.SendingDomain
                                      };
            var emailClient = new MailgunEmailClient(mailgunClientConfig);
            var response = await emailClient.SendAsync(MailgunMessage);
            return response;
        }
    }
}
