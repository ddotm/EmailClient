using DdotM.EmailClient.Mailgun;
using DdotM.EmailClient.Office365;
using EmailMessage = DdotM.EmailClient.Office365.EmailMessage;

namespace EmailClientTester;

internal static class Program
{
    private static EmailMessage EmailMessage { get; set; } = new();
    private static MailgunMessage MailgunMessage { get; set; } = new();
    private static Office365ClientConfig Office365ClientConfig { get; set; } = new();
    private static MailgunClientConfig MailgunClientConfig { get; set; } = new();

    private static async Task Main(string[] args)
    {
        Office365ClientConfig = new Office365ClientConfig();
        EmailMessage = new EmailMessage();

        MailgunClientConfig = new MailgunClientConfig();
        MailgunMessage = new MailgunMessage();

        // CollectInputForOffice365Email();
        // HardcodeInputForOffice365();

        CollectInputForMailgunEmail();
        // HardcodeInputForMailgun();

        // await TestOffice365Client();
        var response = await TestMailgunClient();
        Console.WriteLine($"Email sent with response code {response.Response.StatusCode}");
    }

    private static void CollectInputForOffice365Email()
    {
        Console.WriteLine($"Sender name: ");
        EmailMessage.FromEmail.Name = Console.ReadLine();
        Console.WriteLine($"Sender email address: ");
        EmailMessage.FromEmail.Address = Console.ReadLine();
        Console.WriteLine($"Sender password (for {EmailMessage.FromEmail.Address})");
        Office365ClientConfig.Id = EmailMessage.FromEmail.Address;
        Office365ClientConfig.Pwd = Console.ReadLine();

        EmailMessage.BccEmails.Add(new EmailRecipient());
        Console.WriteLine($"Name of recipient:");
        EmailMessage.BccEmails[0].Name = Console.ReadLine();
        Console.WriteLine($"Recipient email address:");
        EmailMessage.BccEmails[0].Address = Console.ReadLine();

        Console.WriteLine($"Email subject:");
        EmailMessage.Subject = Console.ReadLine();
        Console.WriteLine($"Email text:");
        EmailMessage.TextBody = Console.ReadLine();
        Console.Clear();
    }

    private static void CollectInputForMailgunEmail()
    {
        Console.WriteLine($"Sender name: ");
        MailgunMessage.From.Name = Console.ReadLine();
        Console.WriteLine($"Sender email address: ");
        MailgunMessage.From.Address = Console.ReadLine();

        Console.WriteLine($"Mailgun API key:");
        MailgunClientConfig.ApiKey = Console.ReadLine();
        Console.WriteLine($"Mailgun sending domain:");
        MailgunClientConfig.SendingDomain = Console.ReadLine();

        MailgunMessage.ToEmails.Add(new Recipient());
        Console.WriteLine($"Name of recipient:");
        MailgunMessage.ToEmails[0].Name = Console.ReadLine();
        Console.WriteLine($"Recipient email address:");
        MailgunMessage.ToEmails[0].Address = Console.ReadLine();

        Console.WriteLine($"Email subject:");
        MailgunMessage.Subject = Console.ReadLine();
        Console.WriteLine($"Email text:");
        MailgunMessage.TextBody = Console.ReadLine();
        Console.Clear();
    }

    private static void HardcodeInputForOffice365()
    {
        EmailMessage.FromEmail.Name = "";
        EmailMessage.FromEmail.Address = "";

        Office365ClientConfig.Id = EmailMessage.FromEmail.Address;
        Office365ClientConfig.Pwd = "";

        EmailMessage.BccEmails.Add(new EmailRecipient
        {
            Name = "",
            Address = ""
        });

        EmailMessage.Subject = "Test message subject";
        EmailMessage.TextBody = "Test message text";
        EmailMessage.HtmlBody = $"<html><body><p>{EmailMessage.TextBody}</p></body></html>";
    }

    private static void HardcodeInputForMailgun()
    {
        MailgunClientConfig.ApiKey = "";
        MailgunClientConfig.SendingDomain = "";
        MailgunClientConfig.RequireTls = true;
        MailgunClientConfig.SkipVerification = false;

        MailgunMessage.From.Name = "";
        MailgunMessage.From.Address = "";

        MailgunMessage.BccEmails.Add(new Recipient
        {
            Name = "",
            Address = ""
        });

        MailgunMessage.Subject = "Test message subject";
        MailgunMessage.TextBody = "Test message text";
        MailgunMessage.HtmlBody = $"<html><body><p>{MailgunMessage.TextBody}</p></body></html>";

        MailgunMessage.Tags.Add("registration");
        MailgunMessage.Tracking = false;
        MailgunMessage.DeliveryTime = null;
    }

    private static async Task TestOffice365Client()
    {
        var office365ClientConfig = new Office365ClientConfig
        {
            Id = Office365ClientConfig.Id,
            Pwd = Office365ClientConfig.Pwd
        };
        await using var office365Client = new Office365EmailClient(office365ClientConfig);
        await office365Client.SendAsync(EmailMessage);
    }

    private static async Task<MailgunMessage> TestMailgunClient()
    {
        var mailgunClientConfig = new MailgunClientConfig
        {
            ApiKey = MailgunClientConfig.ApiKey,
            SendingDomain = MailgunClientConfig.SendingDomain,
            RequireTls = true,
            SkipVerification = false
        };

        var mailgunClient = new MailgunClient(mailgunClientConfig);
        var response = await mailgunClient.SendAsync(MailgunMessage);
        return response;
    }
}