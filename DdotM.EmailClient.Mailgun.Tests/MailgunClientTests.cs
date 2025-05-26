using System.ComponentModel.DataAnnotations;
using FluentAssertions;
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
}
