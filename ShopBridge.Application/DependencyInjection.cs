
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShopBridge.Application.Common.Behaviors;
using ShopBridge.Application.Common.Mapping;

namespace ShopBridge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        MappingConfig.RegisterMappings();

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
