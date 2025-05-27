using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using AwesomeAssertions;
using NSubstitute;

namespace DdotM.EmailClient.Mailgun.Tests;

public class MailgunClientTests
{
    [Fact]
    public void MailgunClient_Ctor_NullConfig_Throws()
    {
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        Action act = () => new MailgunClient(null!, adapter, requestBuilder);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*mailgunClientConfig*");
    }

    [Fact]
    public void MailgunClient_Ctor_NullAdapter_Throws()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        Action act = () => new MailgunClient(config, null!, requestBuilder);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*httpClientAdapter*");
    }

    [Fact]
    public void MailgunClient_Ctor_NullRequestBuilder_Throws()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();

        Action act = () => new MailgunClient(config, adapter, null!);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*requestBuilder*");
    }

    [Fact]
    public void MailgunClient_Ctor_InvalidConfig_Throws()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        Action act = () => new MailgunClient(config, adapter, requestBuilder);

        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void MailgunClient_Ctor_ValidConfig_Success()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        var client = new MailgunClient(config, adapter, requestBuilder);

        client.Should().NotBeNull();
    }

    [Fact]
    public async Task MailgunClient_SendAsync_Success_ReturnsResponse()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "my-test-api-key",
            SendingDomain = "mg.example.com"
        };
        var expectedEndpoint = config.MailgunApiEndpoint;

        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        var testMessage = new MailgunMessage();
        var testContent = new StringContent("test");

        requestBuilder.Build(testMessage).Returns(testContent);

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK!")
        };

        adapter.PostAsync(expectedEndpoint, testContent).Returns(Task.FromResult(httpResponse));

        var client = new MailgunClient(config, adapter, requestBuilder);

        // Act
        var result = await client.SendAsync(testMessage);

        // Assert
        // Check Authorization header is set with "Basic {base64}" pattern
        adapter.Received(1).AddHeader("Authorization", Arg.Is<string>(value =>
            value.StartsWith("Basic ") &&
            // Decode and verify credentials are correct
            Encoding.UTF8.GetString(Convert.FromBase64String(value.Substring("Basic ".Length)))
                == "api:my-test-api-key"
        ));

        // PostAsync should have been called with correct endpoint and content
        await adapter.Received(1).PostAsync(expectedEndpoint, testContent);

        result.Should().NotBeNull();
        result.Response.Should().Be(httpResponse);
    }

    [Fact]
    public async Task MailgunClient_SendAsync_NonSuccess_ThrowsHttpRequestException()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "fail-key",
            SendingDomain = "mg.example.com"
        };
        var adapter = Substitute.For<IHttpClientAdapter>();
        var requestBuilder = Substitute.For<IMailgunRequestBuilder>();

        var testMessage = new MailgunMessage();
        var testContent = new StringContent("fail-content");
        requestBuilder.Build(testMessage).Returns(testContent);

        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Bad request test error")
        };

        adapter.PostAsync(config.MailgunApiEndpoint, testContent).Returns(Task.FromResult(httpResponse));

        var client = new MailgunClient(config, adapter, requestBuilder);

        // Act
        Func<Task> act = async () => await client.SendAsync(testMessage);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Mailgun API request failed with status code BadRequest: Bad request test error");

        adapter.Received(1).AddHeader("Authorization", Arg.Any<string>());
        await adapter.Received(1).PostAsync(config.MailgunApiEndpoint, testContent);
    }
}
