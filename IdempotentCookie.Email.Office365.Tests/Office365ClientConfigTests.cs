using System.ComponentModel.DataAnnotations;
using AwesomeAssertions;
using Xunit;

namespace IdempotentCookie.Email.Office365.Tests;

public class Office365ClientConfigTests
{
    [Fact]
    public void Office365ClientConfig_Validate_WhenConfigurationIsValid_DoesNotThrow()
    {
        // Arrange
        var config = new Office365ClientConfig
        {
            Id = "sender@example.com",
            Pwd = "secret"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Office365ClientConfig_Validate_WhenIdIsMissing_ThrowsValidationException()
    {
        // Arrange
        var config = new Office365ClientConfig
        {
            Pwd = "secret"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*Id*");
    }

    [Fact]
    public void Office365ClientConfig_CreateClient_WhenCalled_ReturnsOffice365Provider()
    {
        // Arrange
        var config = new Office365ClientConfig
        {
            Id = "sender@example.com",
            Pwd = "secret"
        };

        // Act
        var client = config.CreateClient();

        // Assert
        client.Provider.Should().Be(EmailProvider.Office365);
    }
}