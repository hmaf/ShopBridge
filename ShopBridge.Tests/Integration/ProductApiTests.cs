
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.DTOs;
using ShopBridge.Domain.Entities;
using ShopBridge.Infra.Persistence;
using ShopBridge.Tests.Integration.Infrastructure;
using Xunit;

namespace ShopBridge.Tests.Integration;

public class ProductApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_Should_Return_SuccessfulResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/Product");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<List<ProductDto>>>();

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetProductById_Should_Return_Product_When_Exists()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var product = new Product(
            "Integration Test Product",
            125.50m,
            10);

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var response = await _client.GetAsync(
                $"/api/Product/{product.Id}");

            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse<ProductDto?>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            Assert.Equal(product.Id, result.Data.Id);
            Assert.Equal("Integration Test Product", result.Data.Name);
            Assert.Equal(125.50m, result.Data.Price);
            Assert.Equal(10, result.Data.Stock);
        }
        finally
        {
            // Cleanup
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task CreateProduct_Should_Create_Product_And_Return_CreatedResponse()
    {
        // Arrange
        var command = new
        {
            Name = "Created By Integration Test",
            Price = 250.75m,
            Stock = 20
        };

        Guid productId = Guid.Empty;

        try
        {
            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/Product",
                command);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.True(
    response.StatusCode == HttpStatusCode.Created,
    $"Status: {response.StatusCode}\nResponse Body: {responseBody}");

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse<Guid>>();

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotEqual(Guid.Empty, result.Data);

            productId = result.Data;

            // Verify persisted data through API
            var getResponse = await _client.GetAsync(
                $"/api/Product/{productId}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var productResult = await getResponse.Content
                .ReadFromJsonAsync<ApiResponse<ProductDto?>>();

            Assert.NotNull(productResult);
            Assert.True(productResult.Success);
            Assert.NotNull(productResult.Data);

            Assert.Equal(productId, productResult.Data.Id);
            Assert.Equal(command.Name, productResult.Data.Name);
            Assert.Equal(command.Price, productResult.Data.Price);
            Assert.Equal(command.Stock, productResult.Data.Stock);
        }
        finally
        {
            if (productId != Guid.Empty)
            {
                using var scope = _factory.Services.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                await dbContext.Products
                    .Where(p => p.Id == productId)
                    .ExecuteDeleteAsync();
            }
        }
    }


    [Theory]
    [InlineData("", 100, 10)]
    [InlineData("Valid Product", 0, 10)]
    [InlineData("Valid Product", 100, -1)]
    public async Task CreateProduct_Should_Return_BadRequest_When_Input_Is_Invalid(
        string name,
        decimal price,
        int stock)
    {
        // Arrange
        var command = new
        {
            Name = name,
            Price = price,
            Stock = stock
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            command);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_Update_Product_And_Return_Success()
    {
        // Arrange
        var product = new Product("Old Laptop", 1000, 5);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
        }

        try
        {
            var request = new
            {
                name = "New Laptop",
                price = 1500,
                stock = 10
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"/api/Product/{product.Id}",
                request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var scope = _factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var updatedProduct = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            updatedProduct.Should().NotBeNull();
            updatedProduct!.Name.Should().Be("New Laptop");
            updatedProduct.Price.Should().Be(1500);
            updatedProduct.Stock.Should().Be(10);
        }
        finally
        {
            using var scope = _factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            await dbContext.Products
                .Where(x => x.Id == product.Id)
                .ExecuteDeleteAsync();
        }
    }

    [Theory]
    [InlineData("", 1500, 10)]
    [InlineData("Valid Laptop", 0, 10)]
    [InlineData("Valid Laptop", 1500, -1)]
    public async Task UpdateProduct_Should_Return_BadRequest_For_Invalid_Input(
    string name,
    decimal price,
    int stock)
    {
        // Arrange
        var product = new Product("Old Laptop", 1000, 5);

        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        try
        {
            var request = new
            {
                name,
                price,
                stock
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"/api/Product/{product.Id}",
                request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        finally
        {
            await dbContext.Products
                .Where(x => x.Id == product.Id)
                .ExecuteDeleteAsync();
        }
    }
}
