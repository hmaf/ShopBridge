
namespace ShopBridge.Application.Features.Products.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    int Stock
);
