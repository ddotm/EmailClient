using System.Net;
using AwesomeAssertions;
using IdempotentCookie.Email;
using NSubstitute;
using SendGrid.Helpers.Mail;
using Xunit;

namespace IdempotentCookie.Email.SendGrid.Tests;

public class SendGridEmailClientTests
{
    [Fact]
    public async Task SendGridEmailClient_SendAsync_WhenResponseIsSuccessful_UsesAdapterAndMappedPayload()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var config = new SendGridClientConfig { ApiKey = "SG.test-key" };
        var adapter = Substitute.For<ISendGridClientAdapter>();
        var adapterFactory = Substitute.For<ISendGridClientAdapterFactory>();
        var messageFactory = Substitute.For<ISendGridMessageFactory>();
        var emailMessage = new EmailMessage();
        var sendGridMessage = new SendGridMessage();

        adapterFactory.Create(config).Returns(adapter);
        messageFactory.Create(emailMessage).Returns(sendGridMessage);
        adapter.SendEmailAsync(sendGridMessage, Arg.Any<CancellationToken>())
            .Returns(new SendGridResponse(HttpStatusCode.Accepted, string.Empty));

        var client = new SendGridEmailClient(config, adapterFactory, messageFactory);

        // Act
        await client.SendAsync(emailMessage, cancellationToken);

        // Assert
        adapterFactory.Received(1).Create(config);
        messageFactory.Received(1).Create(emailMessage);
        await adapter.Received(1).SendEmailAsync(sendGridMessage, cancellationToken);
    }

    [Fact]
    public async Task SendGridEmailClient_SendAsync_WhenCancellationTokenIsProvided_ForwardsCancellationToken()
    {
        // Arrange
        var config = new SendGridClientConfig { ApiKey = "SG.test-key" };
        var adapter = Substitute.For<ISendGridClientAdapter>();
        var adapterFactory = Substitute.For<ISendGridClientAdapterFactory>();
        var messageFactory = Substitute.For<ISendGridMessageFactory>();
        var emailMessage = new EmailMessage();
        var sendGridMessage = new SendGridMessage();
        var cancellationToken = new CancellationTokenSource().Token;

        adapterFactory.Create(config).Returns(adapter);
        messageFactory.Create(emailMessage).Returns(sendGridMessage);
        adapter.SendEmailAsync(sendGridMessage, cancellationToken)
            .Returns(new SendGridResponse(HttpStatusCode.Accepted, string.Empty));

        var client = new SendGridEmailClient(config, adapterFactory, messageFactory);

        // Act
        await client.SendAsync(emailMessage, cancellationToken);

        // Assert
        await adapter.Received(1).SendEmailAsync(sendGridMessage, cancellationToken);
    }

    [Fact]
    public async Task SendGridEmailClient_SendAsync_WhenResponseIsUnsuccessful_ThrowsHttpRequestException()
    {
        // Arrange
        var config = new SendGridClientConfig { ApiKey = "SG.test-key" };
        var adapter = Substitute.For<ISendGridClientAdapter>();
        var adapterFactory = Substitute.For<ISendGridClientAdapterFactory>();
        var messageFactory = Substitute.For<ISendGridMessageFactory>();
        var sendGridMessage = new SendGridMessage();

        adapterFactory.Create(config).Returns(adapter);
        messageFactory.Create(Arg.Any<EmailMessage>()).Returns(sendGridMessage);
        adapter.SendEmailAsync(sendGridMessage, Arg.Any<CancellationToken>())
            .Returns(new SendGridResponse(HttpStatusCode.BadRequest, "bad request"));

        var client = new SendGridEmailClient(config, adapterFactory, messageFactory);

        // Act
        Func<Task> act = () => client.SendAsync(new EmailMessage());

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("SendGrid API request failed with status code BadRequest: bad request");
    }

    [Fact]
    public void SendGridMessageFactory_Create_WhenMessageHasRecipientsAndAttachments_MapsSendGridMessage()
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
            ContentType = "text/plain",
            Content = "hello"u8.ToArray()
        });

        var factory = new SendGridMessageFactory();

        // Act
        var result = factory.Create(message);

        // Assert
        result.From.Email.Should().Be("sender@example.com");
        result.Personalizations.Should().ContainSingle();
        result.Attachments.Should().ContainSingle();
        result.Subject.Should().Be("Subject");
    }
}