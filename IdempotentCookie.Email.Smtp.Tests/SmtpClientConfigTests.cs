using System.ComponentModel.DataAnnotations;
using AwesomeAssertions;
using Xunit;

namespace IdempotentCookie.Email.Smtp.Tests;

public class SmtpClientConfigTests
{
    [Fact]
    public void Validate_WithValidConfig_DoesNotThrow()
    {
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            Port = 587,
            UserName = "mailer",
            Password = "secret"
        };

        Action act = () => config.Validate();

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithoutHost_Throws()
    {
        var config = new SmtpClientConfig();

        Action act = () => config.Validate();

        act.Should().Throw<ValidationException>()
            .WithMessage("*Host*");
    }

    [Fact]
    public void Validate_WithPartialCredentials_Throws()
    {
        var config = new SmtpClientConfig
        {
            Host = "smtp.example.com",
            UserName = "mailer"
        };

        Action act = () => config.Validate();

        act.Should().Throw<ValidationException>()
            .WithMessage("*UserName and Password must both be provided*");
    }
}