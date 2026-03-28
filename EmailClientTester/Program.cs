using IdempotentCookie.Email;
using IdempotentCookie.Email.Mailgun;
using IdempotentCookie.Email.Office365;

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
        EmailMessage.From.Name = ReadInput();
        Console.WriteLine($"Sender email address: ");
        EmailMessage.From.Address = ReadInput();
        Console.WriteLine($"Sender password (for {EmailMessage.From.Address})");
        Office365ClientConfig.Id = EmailMessage.From.Address;
        Office365ClientConfig.Pwd = ReadInput();

        EmailMessage.BccRecipients.Add(new EmailAddress());
        Console.WriteLine($"Name of recipient:");
        EmailMessage.BccRecipients[0].Name = ReadInput();
        Console.WriteLine($"Recipient email address:");
        EmailMessage.BccRecipients[0].Address = ReadInput();

        Console.WriteLine($"Email subject:");
        EmailMessage.Subject = ReadInput();
        Console.WriteLine($"Email text:");
        EmailMessage.TextBody = ReadInput();
        Console.Clear();
    }

    private static void CollectInputForMailgunEmail()
    {
        Console.WriteLine($"Sender name: ");
        MailgunMessage.From.Name = ReadInput();
        Console.WriteLine($"Sender email address: ");
        MailgunMessage.From.Address = ReadInput();

        Console.WriteLine($"Mailgun API key:");
        MailgunClientConfig.ApiKey = ReadInput();
        Console.WriteLine($"Mailgun sending domain:");
        MailgunClientConfig.SendingDomain = ReadInput();

        MailgunMessage.ToEmails.Add(new Recipient());
        Console.WriteLine($"Name of recipient:");
        MailgunMessage.ToEmails[0].Name = ReadInput();
        Console.WriteLine($"Recipient email address:");
        MailgunMessage.ToEmails[0].Address = ReadInput();

        Console.WriteLine($"Email subject:");
        MailgunMessage.Subject = ReadInput();
        Console.WriteLine($"Email text:");
        MailgunMessage.TextBody = ReadInput();
        Console.Clear();
    }

    private static string ReadInput()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    private static void HardcodeInputForOffice365()
    {
        EmailMessage.From.Name = "";
        EmailMessage.From.Address = "";

        Office365ClientConfig.Id = EmailMessage.From.Address;
        Office365ClientConfig.Pwd = "";

        EmailMessage.BccRecipients.Add(new EmailAddress
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
        var office365Client = new Office365EmailClient(office365ClientConfig);
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