using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Products.Command;
using Products.Repository;
using MongoDB.Driver;
using IntegratedTests.Fixtures;
using Infrastructure.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Products.Repository.Interface; // Para o MongoDbFixture

namespace IntegratedTests
{
    public class ProductControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<MongoDbFixture>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly MongoDbFixture _mongoDbFixture;

        public ProductControllerIntegrationTests(WebApplicationFactory<Program> factory, MongoDbFixture mongoDbFixture)
        {
            _factory = factory;
            _mongoDbFixture = mongoDbFixture;
        }

        [Fact(DisplayName = "POST /Product should create product successfully")]
        public async Task CreateProduct_ShouldCreateProductSuccessfully()
        {
            // Arrange
            var command = new CreateProductCommand(Guid.NewGuid(), "Product1", "Type1", 100, 10, DateTime.UtcNow.AddDays(30), 1);
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IProductRepository>(_ =>
                        new ProductRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Products")); // Nome da coleção
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
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IProductRepository>(_ =>
                        new ProductRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Products")); // Nome da coleção
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
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IProductRepository>(_ =>
                        new ProductRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Products")); // Nome da coleção
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/Product");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseProducts = JsonConvert.DeserializeObject<List<ProductDomain>>(responseString);
            Assert.NotNull(responseProducts);
            Assert.NotEmpty(responseProducts);
        }

        [Fact(DisplayName = "GET /Product should return product by query")]
        public async Task GetByProduct_ShouldReturnProductByQuery()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IProductRepository>(_ =>
                        new ProductRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Products")); // Nome da coleção
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync($"/Product?productId={productId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseProduct = JsonConvert.DeserializeObject<ProductDomain>(responseString);
            Assert.NotNull(responseProduct);
            Assert.Equal(productId, responseProduct.Id);
        }

        [Fact(DisplayName = "GET /Product/statement should return product statement by query")]
        public async Task GetStatementByProduct_ShouldReturnProductStatementByQuery()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IProductRepository>(_ =>
                        new ProductRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "ProductStatements")); // Nome da coleção de statements
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync($"/Product/statement?productId={productId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseStatements = JsonConvert.DeserializeObject<List<ProductDomain>>(responseString);
            Assert.NotNull(responseStatements);
            Assert.NotEmpty(responseStatements);
        }

        [Fact(DisplayName = "DELETE /Product/{id} should delete product successfully")]
        public async Task Delete_ShouldDeleteProductSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IProductRepository>(_ =>
                        new ProductRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Products")); // Nome da coleção
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
