
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopBridge.Application.Abstractions.Logging;
using ShopBridge.Application.Abstractions.Persistence;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Infra.Logging.Metadata;
using ShopBridge.Infra.Logging.Services;
using ShopBridge.Infra.Persistence;
using ShopBridge.Infra.Repositories;

namespace ShopBridge.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var defaultConnectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(defaultConnectionString));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var logsConnectionString =
            configuration.GetConnectionString("LogsConnection")
            ?? throw new InvalidOperationException(
                "LogsConnection is not configured.");

        services.AddSingleton(
            new LogMetadataInitializer(logsConnectionString));

        services.AddSingleton<ILogMetadataResolver>(
            new LogMetadataResolver(logsConnectionString));

        services.AddHttpContextAccessor();
        services.AddScoped<ILogService, LogService>();

        return services;
    }
}
