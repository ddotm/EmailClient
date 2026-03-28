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
    public async Task SendAsync_OnSuccess_UsesAdapterAndMappedPayload()
    {
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

        await client.SendAsync(emailMessage);

        adapterFactory.Received(1).Create(config);
        messageFactory.Received(1).Create(emailMessage);
        await adapter.Received(1).SendEmailAsync(sendGridMessage, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_OnFailure_ThrowsHttpRequestException()
    {
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

        Func<Task> act = () => client.SendAsync(new EmailMessage());

        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("SendGrid API request failed with status code BadRequest: bad request");
    }

    [Fact]
    public void MessageFactory_MapsRecipientsAndAttachments()
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

        var factory = new SendGridMessageFactory();

        var result = factory.Create(message);

        result.From.Email.Should().Be("sender@example.com");
        result.Personalizations.Should().ContainSingle();
        result.Attachments.Should().ContainSingle();
        result.Subject.Should().Be("Subject");
    }
}