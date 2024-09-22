using System.Reflection;
using MassTransit;

namespace VerticalSliceTemplate.Api.Shared.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterMessaging(this IServiceCollection services, Assembly consumersAssembly)
    {
        services.AddMassTransit(options =>
        {
            options.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter('.', "template", false));
            options.AddConsumers(consumersAssembly);

            options.UsingInMemory((context, config) =>
            {
                config.AutoStart = true;
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}