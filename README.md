# IdempotentCookie.Email

Unified email delivery package for .NET.  
Supports SMTP, Mailgun, and SendGrid through one public NuGet package.  
Configure one provider per application. No failover is built in.

## Supported Providers

- SMTP
- Mailgun
- SendGrid

## Install

1. Run `dotnet add package IdempotentCookie.Email`.
2. Import `IdempotentCookie.Email` plus the provider namespace you need.

If you bind provider settings from configuration outside an ASP.NET Core Web SDK app, also add:

- `dotnet add package Microsoft.Extensions.Configuration.Binder`
- `dotnet add package Microsoft.Extensions.Options.ConfigurationExtensions`
- `dotnet add package Microsoft.Extensions.Options.DataAnnotations`

## Register With Dependency Injection

1. Import the DI namespace and one provider namespace.
2. Register exactly one provider in `Program.cs`.
3. Resolve `IEmailClient` where you need to send mail.

```csharp
using IdempotentCookie.Email.DependencyInjection;
using IdempotentCookie.Email.Smtp;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddEmailSending()
  .UseSmtp(new SmtpClientConfig
  {
    Host = "smtp.example.com",
    Port = 587,
    Security = SmtpConnectionSecurity.StartTls,
    UserName = "mailer",
    Password = "secret"
  });
```

If you prefer to hydrate SMTP settings from configuration, bind the section once during startup, validate it, then pass the bound config into `UseSmtp(...)`. The library does not define a built-in section name, so pick one such as `Email:Smtp`.

```csharp
using IdempotentCookie.Email.DependencyInjection;
using IdempotentCookie.Email.Smtp;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
const string smtpSectionName = "Email:Smtp";

builder.Services
  .AddOptions<SmtpClientConfig>()
  .Bind(builder.Configuration.GetRequiredSection(smtpSectionName))
  .ValidateDataAnnotations()
  .ValidateOnStart();

var smtpConfig = new SmtpClientConfig();
builder.Configuration.GetRequiredSection(smtpSectionName).Bind(smtpConfig);
smtpConfig.Validate();

builder.Services
  .AddEmailSending()
  .UseSmtp(smtpConfig);
```

## Send With Dependency Injection

1. Inject `IEmailClient`.
2. Build an `EmailMessage`.
3. Call `SendAsync`.

```csharp
using IdempotentCookie.Email;

public sealed class WelcomeEmailSender(IEmailClient emailClient)
{
  public async Task SendWelcomeEmailAsync(CancellationToken cancellationToken)
  {
    var message = new EmailMessage
    {
      From = new EmailAddress { Address = "sender@example.com", Name = "Sender" },
      Subject = "Hello",
      TextBody = "Hello from IdempotentCookie.Email",
      HtmlBody = "<p>Hello from IdempotentCookie.Email</p>"
    };

    message.ToRecipients.Add(new EmailAddress
    {
      Address = "recipient@example.com",
      Name = "Recipient"
    });

    await emailClient.SendAsync(message, cancellationToken);
  }
}
```

## Use Without Dependency Injection

1. Create a provider configuration object.
2. Call `CreateClient()`.
3. Send the message through `IEmailClient`.

```csharp
using IdempotentCookie.Email;
using IdempotentCookie.Email.Mailgun;

public sealed class MailgunSmokeTest
{
  public async Task RunAsync(CancellationToken cancellationToken)
  {
    var config = new MailgunClientConfig
    {
      ApiKey = "key-...",
      SendingDomain = "mg.example.com"
    };

    var message = new EmailMessage
    {
      From = new EmailAddress { Address = "sender@example.com", Name = "Sender" },
      Subject = "Hello",
      TextBody = "Hello from IdempotentCookie.Email"
    };

    message.ToRecipients.Add(new EmailAddress { Address = "recipient@example.com", Name = "Recipient" });

    IEmailClient client = config.CreateClient();
    await client.SendAsync(message, cancellationToken);
  }
}
```

## Provider Namespaces

- `IdempotentCookie.Email.DependencyInjection`
- `IdempotentCookie.Email.Smtp`
- `IdempotentCookie.Email.Mailgun`
- `IdempotentCookie.Email.SendGrid`
