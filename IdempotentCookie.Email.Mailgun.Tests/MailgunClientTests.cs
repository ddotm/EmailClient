using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using AwesomeAssertions;
using IdempotentCookie.Email;
using NSubstitute;

namespace IdempotentCookie.Email.Mailgun.Tests;

public class MailgunClientTests
{
    [Fact]
    public void MailgunClient_Constructor_WhenConfigIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        // Act
        Action act = () => new MailgunClient(null!, adapter, requestBuilder);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*mailgunClientConfig*");
    }

    [Fact]
    public void MailgunClient_Constructor_WhenAdapterIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        // Act
        Action act = () => new MailgunClient(config, null!, requestBuilder);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*httpClientAdapter*");
    }

    [Fact]
    public void MailgunClient_Constructor_WhenRequestBuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();

        // Act
        Action act = () => new MailgunClient(config, adapter, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*requestBuilder*");
    }

    [Fact]
    public void MailgunClient_Constructor_WhenConfigurationIsInvalid_ThrowsValidationException()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        // Act
        Action act = () => new MailgunClient(config, adapter, requestBuilder);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void MailgunClient_Constructor_WhenConfigurationIsValid_CreatesInstance()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        // Act
        var client = new MailgunClient(config, adapter, requestBuilder);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public async Task MailgunClient_SendAsync_WhenResponseIsSuccessful_PostsBuiltRequest()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var config = new MailgunClientConfig
        {
            ApiKey = "my-test-api-key",
            SendingDomain = "mg.example.com"
        };
        var expectedEndpoint = config.MailgunApiEndpoint;

        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        var testMessage = new EmailMessage();
        var testContent = new StringContent("test");

        requestBuilder.Build(testMessage).Returns(testContent);

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK!")
        };

        adapter.PostAsync(expectedEndpoint, testContent, Arg.Any<CancellationToken>()).Returns(Task.FromResult(httpResponse));

        var client = new MailgunClient(config, adapter, requestBuilder);

        // Act
        await client.SendAsync(testMessage, cancellationToken);

        // Assert
        // Check Authorization header is set with "Basic {base64}" pattern
        adapter.Received(1).AddHeader("Authorization", Arg.Is<string>(value =>
            value.StartsWith("Basic ") &&
            // Decode and verify credentials are correct
            Encoding.UTF8.GetString(Convert.FromBase64String(value.Substring("Basic ".Length)))
                == "api:my-test-api-key"
        ));

        // PostAsync should have been called with correct endpoint and content
        await adapter.Received(1).PostAsync(expectedEndpoint, testContent, cancellationToken);
    }

    [Fact]
    public async Task MailgunClient_SendAsync_WhenResponseIsUnsuccessful_ThrowsHttpRequestException()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var config = new MailgunClientConfig
        {
            ApiKey = "fail-key",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        var testMessage = new EmailMessage();
        var testContent = new StringContent("fail-content");
        requestBuilder.Build(testMessage).Returns(testContent);

        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad request test error")
        };

        adapter.PostAsync(config.MailgunApiEndpoint, testContent, Arg.Any<CancellationToken>()).Returns(Task.FromResult(httpResponse));

        var client = new MailgunClient(config, adapter, requestBuilder);

        // Act
        Func<Task> act = async () => await client.SendAsync(testMessage, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Mailgun API request failed with status code BadRequest: Bad request test error");

        adapter.Received(1).AddHeader("Authorization", Arg.Any<string>());
        await adapter.Received(1).PostAsync(config.MailgunApiEndpoint, testContent, cancellationToken);
    }

    [Fact]
    public async Task MailgunClient_SendAsync_WhenCancellationTokenIsProvided_ForwardsCancellationToken()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();
        var message = new EmailMessage();
        var content = new StringContent("test");
        var cancellationToken = new CancellationTokenSource().Token;

        requestBuilder.Build(message).Returns(content);
        adapter.PostAsync(config.MailgunApiEndpoint, content, cancellationToken)
            .Returns(new HttpResponseMessage(HttpStatusCode.OK));

        var client = new MailgunClient(config, adapter, requestBuilder);

        // Act
        await client.SendAsync(message, cancellationToken);

        // Assert
        await adapter.Received(1).PostAsync(config.MailgunApiEndpoint, content, cancellationToken);
    }
}
