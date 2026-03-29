using Microsoft.Extensions.DependencyInjection;

namespace IdempotentCookie.Email.DependencyInjection;

/// <summary>
/// Builder returned by <see cref="ServiceCollectionExtensions.AddEmailSending"/> for selecting the active email provider.
/// </summary>
public interface IEmailSendingBuilder
{
    /// <summary>
    /// The underlying service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
