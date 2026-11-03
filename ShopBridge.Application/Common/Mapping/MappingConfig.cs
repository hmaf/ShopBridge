
using Mapster;
using ShopBridge.Application.Features.Products.DTOs;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Common.Mapping;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Product, ProductDto>.NewConfig();
    }
}
