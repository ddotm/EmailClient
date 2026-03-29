using AwesomeAssertions;
using IdempotentCookie.Email.DependencyInjection;
using IdempotentCookie.Email.Mailgun;
using IdempotentCookie.Email.SendGrid;
using IdempotentCookie.Email.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace IdempotentCookie.Email.DependencyInjection.Tests;

public class EmailSendingRegistrationTests
{
    private sealed class StubConfiguration(EmailProvider provider) : IEmailClientConfiguration
    {
        public EmailProvider Provider { get; } = provider;

        public void Validate()
        {
        }
    }

    private sealed class StubClient(EmailProvider provider) : IEmailClient
    {
        public EmailProvider Provider { get; } = provider;

        public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void EmailMessage_Properties_WhenConfigured_ExposeConfiguredValues()
    {
        // Arrange
        var from = new EmailAddress { Address = "sender@example.com", Name = "Sender" };
        var to = new EmailAddress { Address = "recipient@example.com", Name = "Recipient" };
        var attachment = new EmailAttachment
        {
            FileName = "hello.txt",
            ContentType = "text/plain",
            Content = "hello"u8.ToArray()
        };
        var message = new EmailMessage
        {
            From = from,
            Subject = "Subject",
            TextBody = "Text",
            HtmlBody = "<p>Text</p>"
        };
        message.ToRecipients.Add(to);
        message.Attachments.Add(attachment);

        // Act

        // Assert
        message.From.Should().BeSameAs(from);
        message.ToRecipients.Should().ContainSingle().Which.Should().BeSameAs(to);
        message.CcRecipients.Should().BeEmpty();
        message.BccRecipients.Should().BeEmpty();
        message.Subject.Should().Be("Subject");
        message.TextBody.Should().Be("Text");
        message.HtmlBody.Should().Be("<p>Text</p>");
        message.Attachments.Should().ContainSingle().Which.Should().BeSameAs(attachment);
        attachment.FileName.Should().Be("hello.txt");
        attachment.ContentType.Should().Be("text/plain");
        attachment.Content.Should().Equal("hello"u8.ToArray());
        from.Address.Should().Be("sender@example.com");
        from.Name.Should().Be("Sender");
    }

    [Fact]
    public async Task EmailSendingStartupValidationHostedService_StartAsync_WhenProviderIsMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEmailSending();

        using var provider = services.BuildServiceProvider();
        var startupValidator = provider.GetRequiredService<IEnumerable<IHostedService>>().Single();

        // Act
        Func<Task> act = () => startupValidator.StartAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*no provider was configured*");
    }

    [Fact]
    public void SmtpEmailSendingBuilderExtensions_UseSmtp_WhenCalled_RegistersSmtpProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            Port = 587
        };

        // Act
        services.AddEmailSending().UseSmtp(config);

        // Assert
        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IEmailClient>().Provider.Should().Be(EmailProvider.Smtp);
        provider.GetRequiredService<IEmailClientConfiguration>().Provider.Should().Be(EmailProvider.Smtp);
        provider.GetRequiredService<SmtpClientConfig>().Should().BeSameAs(config);
    }

    [Fact]
    public void EmailSendingBuilderRegistrationExtensions_RegisterProvider_WhenSecondProviderIsConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddEmailSending().UseMailgun(new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        });

        // Act
        Action act = () => services.AddEmailSending().UseSendGrid(new SendGridClientConfig
        {
            ApiKey = "SG.test-key"
        });

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only one email provider can be configured*Mailgun*");
    }

    [Theory]
    [InlineData(EmailProvider.Mailgun)]
    [InlineData(EmailProvider.SendGrid)]
    public async Task EmailSendingStartupValidationHostedService_StartAsync_WhenProviderIsConfigured_CompletesSuccessfully(EmailProvider provider)
    {
        // Arrange
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
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
        }

        using var serviceProvider = services.BuildServiceProvider();
        var startupValidator = serviceProvider.GetRequiredService<IEnumerable<IHostedService>>().Single();

        // Act
        Func<Task> act = () => startupValidator.StartAsync(CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EmailSendingStartupValidationHostedService_StartAsync_WhenProviderAndClientDoNotMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEmailSending();
        services.AddSingleton<IEmailClientConfiguration>(new StubConfiguration(EmailProvider.Mailgun));
        services.AddSingleton<IEmailClient>(new StubClient(EmailProvider.SendGrid));

        using var serviceProvider = services.BuildServiceProvider();
        var startupValidator = serviceProvider.GetRequiredService<IEnumerable<IHostedService>>().Single();

        // Act
        Func<Task> act = () => startupValidator.StartAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*does not match the registered client provider*");
        await startupValidator.StopAsync(CancellationToken.None);
    }
}