using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Investments.Controllers;
using Products.Command;
using Products.Query;
using Products.Repository.Interface;
using Infrastructure.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Testing;



namespace FunctionalTests
{

    public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public ProductControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mediatorMock = new Mock<IMediator>();
        _productRepositoryMock = new Mock<IProductRepository>();
    }

    [Fact(DisplayName = "POST /Product should create product successfully")]
    public async Task CreateProduct_ShouldCreateProductSuccessfully()
    {
        // Arrange
        var command = new CreateProductCommand(Guid.NewGuid(), "Product1", "Type1", 100, 10, DateTime.UtcNow.AddDays(30), 1);
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Produto criado com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/Product", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Produto criado com sucesso", responseString);
    }

    [Fact(DisplayName = "PUT /Product should update product successfully")]
    public async Task UpdateProduct_ShouldUpdateProductSuccessfully()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "Product1", "Type1", 100, 10, DateTime.UtcNow.AddDays(30), 1);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Produto atualizado com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/Product", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Produto atualizado com sucesso", responseString);
    }

    [Fact(DisplayName = "GET /Product should return all products")]
    public async Task GetAll_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<ProductDomain>
        {
            new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow.AddDays(30), 1),
            new ProductDomain(Guid.NewGuid(), "Product2", 200, 20, "Type2", DateTime.UtcNow.AddDays(60), 2)
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/Product");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseProducts = JsonConvert.DeserializeObject<List<ProductDomain>>(responseString);
        Assert.Equal(products.ToString(), responseProducts.ToString());
    }

    [Fact(DisplayName = "GET /Product/statement should return product statement")]
    public async Task GetStatementByName_ShouldReturnProductStatement()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var statements = new List<ProductDomain>
        {
            new ProductDomain(productId, "Product1", 100, 10, "Type1", DateTime.UtcNow.AddDays(30), 1),
            new ProductDomain(Guid.NewGuid(), "Product2", 200, 20, "Type2", DateTime.UtcNow.AddDays(60), 2)
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetStatementByProductQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statements);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync($"/Product/statement?productId={productId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseStatements = JsonConvert.DeserializeObject<List<ProductDomain>>(responseString);
        Assert.Equal(statements.ToString(), responseStatements.ToString());
    }

    [Fact(DisplayName = "DELETE /Product/{id} should delete product successfully")]
    public async Task Delete_ShouldDeleteProductSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Produto excluído com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.DeleteAsync($"/Product/{productId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Produto excluído com sucesso", responseString);
    }
}
}

