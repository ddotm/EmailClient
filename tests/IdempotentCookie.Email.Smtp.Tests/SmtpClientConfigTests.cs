using System.ComponentModel.DataAnnotations;
using AwesomeAssertions;
using Xunit;

namespace IdempotentCookie.Email.Smtp.Tests;

public class SmtpClientConfigTests
{
    [Fact]
    public void SmtpClientConfig_Validate_WhenConfigurationIsValid_DoesNotThrow()
    {
        // Arrange
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            Port = 587,
            UserName = "mailer",
            Password = "secret"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void SmtpClientConfig_Validate_WhenHostIsMissing_ThrowsValidationException()
    {
        // Arrange
        var config = new SmtpClientConfig();

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*Host*");
    }

    [Fact]
    public void SmtpClientConfig_Validate_WhenCredentialsArePartial_ThrowsValidationException()
    {
        // Arrange
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            UserName = "mailer"
        };

        // Act
        Action act = () => config.Validate();

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("*UserName and Password must both be provided*");
    }

    [Fact]
    public void SmtpClientConfig_CreateClient_WhenCalled_ReturnsSmtpProvider()
    {
        // Arrange
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com"
        };

        // Act
        var client = config.CreateClient();

        // Assert
        client.Provider.Should().Be(EmailProvider.Smtp);
    }
}