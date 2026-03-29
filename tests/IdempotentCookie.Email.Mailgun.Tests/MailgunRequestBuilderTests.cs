using AwesomeAssertions;
using IdempotentCookie.Email;
using Xunit;

namespace IdempotentCookie.Email.Mailgun.Tests;

public class MailgunRequestBuilderTests
{
    [Fact]
    public async Task MailgunRequestBuilder_Build_WhenMessageHasNoAttachments_ReturnsFormUrlEncodedContent()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com",
            RequireTls = true,
            SkipVerification = false
        };
        var message = new EmailMessage
        {
            From = new EmailAddress { Address = "sender@example.com", Name = "Sender" },
            Subject = "Subject",
            TextBody = "Text"
        };
        message.ToRecipients.Add(new EmailAddress { Address = "to@example.com", Name = "To" });

        var builder = new MailgunRequestBuilder(config);

        // Act
        var content = builder.Build(message);
        var body = await content.ReadAsStringAsync(cancellationToken);

        // Assert
        content.Should().BeOfType<FormUrlEncodedContent>();
        body.Should().Contain("from=Sender+%3Csender%40example.com%3E");
        body.Should().Contain("to=To+%3Cto%40example.com%3E");
        body.Should().Contain("subject=Subject");
        body.Should().Contain("text=Text");
        body.Should().Contain("html=Text");
        body.Should().Contain("o%3Arequire-tls=yes");
        body.Should().Contain("o%3Askip-verification=no");
    }

    [Fact]
    public async Task MailgunRequestBuilder_Build_WhenMessageHasAttachments_ReturnsMultipartContent()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var message = new EmailMessage
        {
            From = new EmailAddress { Address = "sender@example.com" },
            Subject = "Subject",
            TextBody = "Text",
            HtmlBody = "<p>Text</p>"
        };
        message.Attachments.Add(new EmailAttachment
        {
            FileName = "hello.txt",
            ContentType = "text/plain",
            Content = "hello"u8.ToArray()
        });

        var builder = new MailgunRequestBuilder(config);

        // Act
        var content = builder.Build(message);
        var body = await content.ReadAsStringAsync(cancellationToken);

        // Assert
        content.Should().BeOfType<MultipartFormDataContent>();
        body.Should().Contain("form-data; name=attachment; filename=hello.txt");
        body.Should().Contain("Content-Type: text/plain");
        body.Should().Contain("hello");
        body.Should().Contain("<p>Text</p>");
    }

    [Fact]
    public void MailgunClientConfig_CreateClient_WhenCalled_ReturnsMailgunProvider()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };

        // Act
        var client = config.CreateClient();

        // Assert
        client.Provider.Should().Be(EmailProvider.Mailgun);
    }
}