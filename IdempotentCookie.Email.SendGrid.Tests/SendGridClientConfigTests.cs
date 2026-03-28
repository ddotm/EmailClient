using System.ComponentModel.DataAnnotations;
using AwesomeAssertions;
using Xunit;

namespace IdempotentCookie.Email.SendGrid.Tests;

public class SendGridClientConfigTests
{
    [Fact]
    public void SendGridClientConfig_Validate_WhenConfigurationIsValid_DoesNotThrow()
    {
        // Arrange
        var config = new SendGridClientConfig
        {
            ApiKey = "SG.test-key"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void SendGridClientConfig_Validate_WhenApiKeyIsMissing_ThrowsValidationException()
    {
        // Arrange
        var config = new SendGridClientConfig();

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*ApiKey*");
    }

    [Fact]
    public void SendGridClientConfig_CreateClient_WhenCalled_ReturnsSendGridProvider()
    {
        // Arrange
        var config = new SendGridClientConfig
        {
            ApiKey = "SG.test-key"
        };

        // Act
        var client = config.CreateClient();

        // Assert
        client.Provider.Should().Be(EmailProvider.SendGrid);
    }
}