using Microsoft.Extensions.DependencyInjection;

namespace IdempotentCookie.Email.DependencyInjection;

internal static class EmailSendingBuilderRegistrationExtensions
{
    internal static IEmailSendingBuilder RegisterProvider<TConfiguration>(
        this IEmailSendingBuilder builder,
        TConfiguration configuration,
        Func<TConfiguration, IEmailClient> createClient)
        where TConfiguration : class, IEmailClientConfiguration
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(createClient);

        configuration.Validate();

        var existingConfiguration = builder.Services
            .LastOrDefault(descriptor => descriptor.ServiceType == typeof(IEmailClientConfiguration))
            ?.ImplementationInstance as IEmailClientConfiguration;

        if (existingConfiguration is not null)
        {
            throw new InvalidOperationException(
                $"Only one email provider can be configured. '{existingConfiguration.Provider}' is already registered.");
        }

        if (builder.Services.Any(descriptor => descriptor.ServiceType == typeof(IEmailClient)))
        {
            throw new InvalidOperationException("Only one email provider can be configured.");
        }

        builder.Services.AddSingleton(configuration);
        builder.Services.AddSingleton<IEmailClientConfiguration>(configuration);
        builder.Services.AddSingleton<IEmailClient>(_ => createClient(configuration));

        return builder;
    }
}