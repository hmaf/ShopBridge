using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.DTOs;
using ShopBridge.Tests.Integration.Infrastructure;
using System.Net.Http.Json;
using Xunit;

namespace ShopBridge.Tests.Integration;

public class ProductApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_Should_Return_SuccessfulResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/Product");

        // Assert
        Assert.Equal(
            System.Net.HttpStatusCode.OK,
            response.StatusCode);

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }
}
