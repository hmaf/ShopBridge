
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShopBridge.Application.Common.Behaviors;

namespace ShopBridge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
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
