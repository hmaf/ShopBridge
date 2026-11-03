using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.DTOs;
using System.Net.Http.Json;
using Xunit;

namespace ShopBridge.Tests.Integration;

public class ProductApiTests
    : IClassFixture<WebApplicationFactory<global::Program>>
{
    private readonly WebApplicationFactory<global::Program> _factory;

    public ProductApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var testSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

                config.AddJsonFile(testSettingsPath, optional: false, reloadOnChange: false);
            });
        });
    }

    [Fact]
    public async Task GetProducts_Should_Return_ExpectedProduct()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Product");
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert: HTTP status
        Assert.Equal(
            System.Net.HttpStatusCode.OK,
            response.StatusCode);

        // Assert: JSON response
        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var products = await response.Content
            .ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();

        Assert.NotNull(products);
    }
}
