using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdempotentCookie.Email.DependencyInjection;

internal sealed class EmailSendingStartupValidationHostedService(IServiceProvider services) : IHostedService
{
    private readonly IServiceProvider _services = services ?? throw new ArgumentNullException(nameof(services));

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var configuration = scope.ServiceProvider.GetService<IEmailClientConfiguration>();
        var client = scope.ServiceProvider.GetService<IEmailClient>();

        if (configuration is null || client is null)
        {
            throw new InvalidOperationException(
                "Email sending was added to the service collection, but no provider was configured. Call services.AddEmailSending().UseSmtp(...), UseMailgun(...), UseSendGrid(...), or UseOffice365(...)."
            );
        }

        configuration.Validate();

        if (configuration.Provider != client.Provider)
        {
            throw new InvalidOperationException(
                $"The configured email provider '{configuration.Provider}' does not match the registered client provider '{client.Provider}'."
            );
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}