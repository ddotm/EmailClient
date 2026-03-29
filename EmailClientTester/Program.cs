using IdempotentCookie.Email;
using IdempotentCookie.Email.Mailgun;
using IdempotentCookie.Email.SendGrid;
using IdempotentCookie.Email.Smtp;

namespace EmailClientTester;

internal static class Program
{
    private static EmailMessage EmailMessage { get; set; } = new();
    private static SmtpClientConfig SmtpClientConfig { get; set; } = new();
    private static MailgunClientConfig MailgunClientConfig { get; set; } = new();
    private static SendGridClientConfig SendGridClientConfig { get; set; } = new();

    private static async Task Main(string[] args)
    {
        var selectedProvider = CollectProviderSelection();

        EmailMessage = new EmailMessage();
        SmtpClientConfig = new SmtpClientConfig();
        MailgunClientConfig = new MailgunClientConfig();
        SendGridClientConfig = new SendGridClientConfig();

        CollectCommonEmailInput();

        switch (selectedProvider)
        {
            case EmailProvider.Smtp:
                CollectInputForSmtp();
                await TestSmtpClient();
                break;
            case EmailProvider.Mailgun:
                CollectInputForMailgun();
                await TestMailgunClient();
                break;
            case EmailProvider.SendGrid:
                CollectInputForSendGrid();
                await TestSendGridClient();
                break;
            default:
                throw new InvalidOperationException("Unknown provider selection.");
        }

        Console.WriteLine("Email sent.");
    }

    private static EmailProvider CollectProviderSelection()
    {
        while (true)
        {
            Console.WriteLine("Select email provider:");
            Console.WriteLine("1. SMTP");
            Console.WriteLine("2. Mailgun");
            Console.WriteLine("3. SendGrid");
            Console.Write("Enter selection (1-3): ");

            var input = ReadInput();
            if (int.TryParse(input, out var providerNumber) &&
                Enum.IsDefined(typeof(EmailProvider), providerNumber) &&
                providerNumber != (int)EmailProvider.Unknown)
            {
                Console.Clear();
                return (EmailProvider)providerNumber;
            }

            Console.WriteLine("Invalid selection. Please enter a number from 1 to 3.");
            Console.WriteLine();
        }
    }

    private static void CollectCommonEmailInput()
    {
        Console.WriteLine($"Sender name: ");
        EmailMessage.From.Name = ReadInput();
        Console.WriteLine($"Sender email address: ");
        EmailMessage.From.Address = ReadInput();

        EmailMessage.ToRecipients.Add(new EmailAddress());
        Console.WriteLine($"Name of recipient:");
        EmailMessage.ToRecipients[0].Name = ReadInput();
        Console.WriteLine($"Recipient email address:");
        EmailMessage.ToRecipients[0].Address = ReadInput();

        Console.WriteLine($"Email subject:");
        EmailMessage.Subject = ReadInput();
        Console.WriteLine($"Email text:");
        EmailMessage.TextBody = ReadInput();
        EmailMessage.HtmlBody = $"<html><body><p>{EmailMessage.TextBody}</p></body></html>";

        CollectOptionalAttachmentInput();
    }

    private static void CollectOptionalAttachmentInput()
    {
        Console.WriteLine("Add an attachment? (y/n):");
        if (!ReadInput().Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        while (true)
        {
            Console.WriteLine("Attachment file path:");
            var filePath = ReadInput();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found. Try again? (y/n):");
                if (!ReadInput().Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                continue;
            }

            var fileName = Path.GetFileName(filePath);
            var content = File.ReadAllBytes(filePath);

            Console.WriteLine("Attachment content type (optional, e.g. application/pdf). Leave empty to auto/default:");
            var contentType = ReadInput();

            EmailMessage.Attachments.Add(new EmailAttachment
            {
                FileName = fileName,
                Content = content,
                ContentType = string.IsNullOrWhiteSpace(contentType) ? string.Empty : contentType
            });

            Console.WriteLine($"Attached file: {fileName}");
            return;
        }
    }

    private static void CollectInputForSmtp()
    {
        Console.WriteLine("SMTP host:");
        SmtpClientConfig.Host = ReadInput();

        Console.WriteLine("SMTP port (default 587):");
        var portInput = ReadInput();
        SmtpClientConfig.Port = int.TryParse(portInput, out var port) ? port : 587;

        Console.WriteLine("SMTP security: 0=Auto, 1=None, 2=SslOnConnect, 3=StartTls, 4=StartTlsWhenAvailable");
        var securityInput = ReadInput();
        if (int.TryParse(securityInput, out var securityNumber) && Enum.IsDefined(typeof(SmtpConnectionSecurity), securityNumber))
        {
            SmtpClientConfig.Security = (SmtpConnectionSecurity)securityNumber;
        }
        else
        {
            SmtpClientConfig.Security = SmtpConnectionSecurity.StartTls;
        }

        Console.WriteLine("SMTP username (leave empty for no authentication):");
        SmtpClientConfig.UserName = ReadInput();
        Console.WriteLine("SMTP password (leave empty for no authentication):");
        SmtpClientConfig.Password = ReadInput();

        Console.Clear();
    }

    private static void CollectInputForMailgun()
    {
        Console.WriteLine($"Mailgun API key:");
        MailgunClientConfig.ApiKey = ReadInput();
        Console.WriteLine($"Mailgun sending domain:");
        MailgunClientConfig.SendingDomain = ReadInput();
        MailgunClientConfig.RequireTls = true;
        MailgunClientConfig.SkipVerification = false;

        Console.Clear();
    }

    private static void CollectInputForSendGrid()
    {
        Console.WriteLine("SendGrid API key:");
        SendGridClientConfig.ApiKey = ReadInput();

        Console.Clear();
    }

    private static string ReadInput()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    private static async Task TestSmtpClient()
    {
        var smtpClientConfig = new SmtpClientConfig
        {
            Host = SmtpClientConfig.Host,
            Port = SmtpClientConfig.Port,
            Security = SmtpClientConfig.Security,
            UserName = SmtpClientConfig.UserName,
            Password = SmtpClientConfig.Password
        };

        var smtpClient = smtpClientConfig.CreateClient();
        await smtpClient.SendAsync(EmailMessage);
    }

    private static async Task TestMailgunClient()
    {
        var mailgunClientConfig = new MailgunClientConfig
        {
            ApiKey = MailgunClientConfig.ApiKey,
            SendingDomain = MailgunClientConfig.SendingDomain,
            RequireTls = true,
            SkipVerification = false
        };

        var mailgunClient = mailgunClientConfig.CreateClient();
        await mailgunClient.SendAsync(EmailMessage);
    }

    private static async Task TestSendGridClient()
    {
        var sendGridClientConfig = new SendGridClientConfig
        {
            ApiKey = SendGridClientConfig.ApiKey
        };

        var sendGridClient = sendGridClientConfig.CreateClient();
        await sendGridClient.SendAsync(EmailMessage);
    }
}