using AwesomeAssertions;
using IdempotentCookie.Email.DependencyInjection;
using IdempotentCookie.Email.Mailgun;
using IdempotentCookie.Email.Office365;
using IdempotentCookie.Email.SendGrid;
using IdempotentCookie.Email.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace IdempotentCookie.Email.DependencyInjection.Tests;

public class EmailSendingRegistrationTests
{
    [Fact]
    public async Task AddEmailSending_WithoutProvider_FailsStartupValidation()
    {
        var services = new ServiceCollection();
        services.AddEmailSending();

        using var provider = services.BuildServiceProvider();
        var startupValidator = provider.GetRequiredService<IEnumerable<IHostedService>>().Single();

        Func<Task> act = () => startupValidator.StartAsync(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*no provider was configured*");
    }

    [Fact]
    public void UseSmtp_RegistersSmtpProvider()
    {
        var services = new ServiceCollection();
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            Port = 587
        };

        services.AddEmailSending().UseSmtp(config);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IEmailClient>().Provider.Should().Be(EmailProvider.Smtp);
        provider.GetRequiredService<IEmailClientConfiguration>().Provider.Should().Be(EmailProvider.Smtp);
        provider.GetRequiredService<SmtpClientConfig>().Should().BeSameAs(config);
    }

    [Fact]
    public void UseMailgun_ThenUseSendGrid_Throws()
    {
        var services = new ServiceCollection();

        services.AddEmailSending().UseMailgun(new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        });

        Action act = () => services.AddEmailSending().UseSendGrid(new SendGridClientConfig
        {
            ApiKey = "SG.test-key"
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only one email provider can be configured*Mailgun*");
    }

    [Theory]
    [InlineData(EmailProvider.Mailgun)]
    [InlineData(EmailProvider.SendGrid)]
    [InlineData(EmailProvider.Office365)]
    public async Task StartupValidation_SucceedsForConfiguredProvider(EmailProvider provider)
    {
        var services = new ServiceCollection();
        services.AddEmailSending();

        switch (provider)
        {
            case EmailProvider.Mailgun:
                services.AddEmailSending().UseMailgun(new MailgunClientConfig
                {
                    ApiKey = "test-key",
                    SendingDomain = "mg.example.com"
                });
                break;
            case EmailProvider.SendGrid:
                services.AddEmailSending().UseSendGrid(new SendGridClientConfig
                {
                    ApiKey = "SG.test-key"
                });
                break;
            case EmailProvider.Office365:
                services.AddEmailSending().UseOffice365(new Office365ClientConfig
                {
                    Id = "sender@example.com",
                    Pwd = "secret"
                });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
        }

        using var serviceProvider = services.BuildServiceProvider();
        var startupValidator = serviceProvider.GetRequiredService<IEnumerable<IHostedService>>().Single();

        Func<Task> act = () => startupValidator.StartAsync(CancellationToken.None);

        await act.Should().NotThrowAsync();
    }
}