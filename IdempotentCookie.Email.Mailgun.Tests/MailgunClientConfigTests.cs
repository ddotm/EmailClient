using System.ComponentModel.DataAnnotations;
using AwesomeAssertions;

namespace IdempotentCookie.Email.Mailgun.Tests;

public class MailgunClientConfigTests
{
    [Fact]
    public void MailgunClientConfig_Validate_WhenConfigurationIsValid_DoesNotThrow()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "mg.example.com"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void MailgunClientConfig_Validate_WhenApiKeyIsMissing_ThrowsValidationException()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "",
            SendingDomain = "mg.example.com"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*ApiKey*");
    }

    [Fact]
    public void MailgunClientConfig_Validate_WhenSendingDomainIsMissing_ThrowsValidationException()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = ""
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*SendingDomain*");
    }

    [Fact]
    public void MailgunClientConfig_Validate_WhenSendingDomainIsInvalid_ThrowsValidationException()
    {
        // Arrange
        var config = new MailgunClientConfig
        {
            ApiKey = "test-key",
            SendingDomain = "not a domain"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*not a valid sending domain*");
    }
}