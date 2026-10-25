
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Infra.Persistence;
using ShopBridge.Infra.Repositories;

namespace ShopBridge.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
