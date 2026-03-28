using AwesomeAssertions;
using IdempotentCookie.Email;
using MailKit.Security;
using MimeKit;
using NSubstitute;
using Xunit;

namespace IdempotentCookie.Email.Smtp.Tests;

public class SmtpEmailClientTests
{
    [Fact]
    public async Task SendAsync_WithCredentials_ConnectsAuthenticatesAndSends()
    {
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            Port = 2525,
            UserName = "mailer",
            Password = "secret",
            Security = SmtpConnectionSecurity.SslOnConnect
        };
        var smtpClient = Substitute.For<ISmtpClientAdapter>();
        var smtpClientFactory = Substitute.For<ISmtpClientAdapterFactory>();
        var mimeMessageFactory = Substitute.For<ISmtpMimeMessageFactory>();
        var message = new EmailMessage();
        var mimeMessage = new MimeMessage();
        var cancellationToken = new CancellationTokenSource().Token;

        smtpClientFactory.Create().Returns(smtpClient);
        mimeMessageFactory.Create(message).Returns(mimeMessage);

        var client = new SmtpEmailClient(config, smtpClientFactory, mimeMessageFactory);

        await client.SendAsync(message, cancellationToken);

        smtpClientFactory.Received(1).Create();
        mimeMessageFactory.Received(1).Create(message);
        await smtpClient.Received(1).ConnectAsync("smtp.example.com", 2525, SecureSocketOptions.SslOnConnect, cancellationToken);
        await smtpClient.Received(1).AuthenticateAsync("mailer", "secret", cancellationToken);
        await smtpClient.Received(1).SendAsync(mimeMessage, cancellationToken);
        await smtpClient.Received(1).DisconnectAsync(true, cancellationToken);
    }

    [Fact]
    public async Task SendAsync_WithoutCredentials_SkipsAuthentication()
    {
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            Port = 587,
            Security = SmtpConnectionSecurity.StartTls
        };
        var smtpClient = Substitute.For<ISmtpClientAdapter>();
        var smtpClientFactory = Substitute.For<ISmtpClientAdapterFactory>();
        var mimeMessageFactory = Substitute.For<ISmtpMimeMessageFactory>();
        var message = new EmailMessage();

        smtpClientFactory.Create().Returns(smtpClient);
        mimeMessageFactory.Create(message).Returns(new MimeMessage());

        var client = new SmtpEmailClient(config, smtpClientFactory, mimeMessageFactory);

        await client.SendAsync(message);

        await smtpClient.DidNotReceive().AuthenticateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await smtpClient.Received(1).SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void MimeMessageFactory_MapsAttachmentsAndRecipients()
    {
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
            ContentType = "text/plain",
            Content = "hello"u8.ToArray()
        });

        var factory = new SmtpMimeMessageFactory();

        var result = factory.Create(message);

        result.From.Count.Should().Be(1);
        result.To.Count.Should().Be(1);
        result.Cc.Count.Should().Be(1);
        result.Bcc.Count.Should().Be(1);
        result.Subject.Should().Be("Subject");
        result.Body.Should().NotBeNull();
    }
}