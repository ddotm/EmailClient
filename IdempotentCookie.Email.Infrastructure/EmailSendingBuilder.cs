using Microsoft.Extensions.DependencyInjection;

namespace IdempotentCookie.Email.DependencyInjection;

internal sealed class EmailSendingBuilder(IServiceCollection services) : IEmailSendingBuilder
{
    public IServiceCollection Services { get; } = services;
}
