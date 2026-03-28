using System.ComponentModel.DataAnnotations;
using AwesomeAssertions;
using Xunit;

namespace IdempotentCookie.Email.SendGrid.Tests;

public class SendGridClientConfigTests
{
    [Fact]
    public void Validate_WithValidConfig_DoesNotThrow()
    {
        var config = new SendGridClientConfig
        {
            ApiKey = "SG.test-key"
        };

        Action act = () => config.Validate();

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithoutApiKey_Throws()
    {
        var config = new SendGridClientConfig();

        Action act = () => config.Validate();

        act.Should().Throw<ValidationException>()
            .WithMessage("*ApiKey*");
    }
}