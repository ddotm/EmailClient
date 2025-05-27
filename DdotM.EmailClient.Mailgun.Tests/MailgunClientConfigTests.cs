using System.ComponentModel.DataAnnotations;
using AwesomeAssertions;

namespace DdotM.EmailClient.Mailgun.Tests;

public class MailgunClientConfigTests
{
    [Fact]
    public void MailgunClientConfig_Validate_ValidConfig_Success()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };

        Action act = () => config.Validate();

        act.Should().NotThrow();
    }

    [Fact]
    public void MailgunClientConfig_Validate_MissingApiKey_Throws()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "",
            SendingDomain = "mg.example.com"
        };

        Action act = () => config.Validate();

        act.Should().Throw<ValidationException>()
            .WithMessage("*ApiKey*");
    }

    [Fact]
    public void MailgunClientConfig_Validate_MissingSendingDomain_Throws()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = ""
        };

        Action act = () => config.Validate();

        act.Should().Throw<ValidationException>()
            .WithMessage("*SendingDomain*");
    }

    [Fact]
    public void MailgunClientConfig_Validate_InvalidSendingDomain_Throws()
    {
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "not a domain"
        };

        Action act = () => config.Validate();

        act.Should().Throw<ValidationException>()
            .WithMessage("*not a valid sending domain*");
    }
}