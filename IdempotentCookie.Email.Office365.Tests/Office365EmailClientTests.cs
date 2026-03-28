using AwesomeAssertions;
using IdempotentCookie.Email;
using MailKit.Security;
using MimeKit;
using NSubstitute;
using Xunit;

namespace IdempotentCookie.Email.Office365.Tests;

public class Office365EmailClientTests
{
    [Fact]
    public async Task Office365EmailClient_SendAsync_WhenConfigurationIsValid_ConnectsAuthenticatesSendsAndDisconnects()
    {
        // Arrange
        var config = new Office365ClientConfig
        {
            Id = "sender@example.com",
            Pwd = "secret"
        };
        var smtpClient = Substitute.For<IOffice365SmtpClientAdapter>();
        var smtpClientFactory = Substitute.For<IOffice365SmtpClientAdapterFactory>();
        var mimeFactory = Substitute.For<IOffice365MimeMessageFactory>();
        var message = new EmailMessage();
        var mimeMessage = new MimeMessage();
        var cancellationToken = new CancellationTokenSource().Token;

        smtpClientFactory.Create().Returns(smtpClient);
        mimeFactory.Create(message).Returns(mimeMessage);

        var client = new Office365EmailClient(config, smtpClientFactory, mimeFactory);

    // Act
        await client.SendAsync(message, cancellationToken);

    // Assert
        await smtpClient.Received(1).ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls, cancellationToken);
        await smtpClient.Received(1).AuthenticateAsync("sender@example.com", "secret", cancellationToken);
        await smtpClient.Received(1).SendAsync(mimeMessage, cancellationToken);
        await smtpClient.Received(1).DisconnectAsync(true, cancellationToken);
    }

    [Fact]
    public async Task Office365EmailClient_SendAsync_WhenSendFails_DisconnectsBeforeThrowing()
    {
        // Arrange
        var config = new Office365ClientConfig
        {
            Id = "sender@example.com",
            Pwd = "secret"
        };
        var smtpClient = Substitute.For<IOffice365SmtpClientAdapter>();
        var smtpClientFactory = Substitute.For<IOffice365SmtpClientAdapterFactory>();
        var mimeFactory = Substitute.For<IOffice365MimeMessageFactory>();
        var message = new EmailMessage();

        smtpClientFactory.Create().Returns(smtpClient);
        mimeFactory.Create(message).Returns(new MimeMessage());
        smtpClient.SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("send failed"));

        var client = new Office365EmailClient(config, smtpClientFactory, mimeFactory);

        // Act
        Func<Task> act = () => client.SendAsync(message, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("send failed");
        await smtpClient.Received(1).DisconnectAsync(true, CancellationToken.None);
    }

    [Fact]
    public void Office365MimeMessageFactory_Create_WhenMessageHasRecipientsAndAttachments_MapsMimeMessage()
    {
        // Arrange
        var message = new EmailMessage
        {
            From = new EmailAddress { Address = "sender@example.com", Name = "Sender" },
            Subject = "Subject",
            TextBody = "Text",
            HtmlBody = "<p>Text</p>"
        };
        message.ToRecipients.Add(new EmailAddress { Address = "to@example.com", Name = "To" });
        message.CcRecipients.Add(new EmailAddress { Address = "cc@example.com", Name = "Cc" });
        message.BccRecipients.Add(new EmailAddress { Address = "bcc@example.com", Name = "Bcc" });
        message.Attachments.Add(new EmailAttachment
        {
            FileName = "hello.txt",
            Content = "hello"u8.ToArray(),
            ContentType = "text/plain"
        });

        var factory = new Office365MimeMessageFactory();

    // Act
        var result = factory.Create(message);

    // Assert
        result.From.Count.Should().Be(1);
        result.To.Count.Should().Be(1);
        result.Cc.Count.Should().Be(1);
        result.Bcc.Count.Should().Be(1);
        result.Subject.Should().Be("Subject");
        result.Body.Should().NotBeNull();
    }
}