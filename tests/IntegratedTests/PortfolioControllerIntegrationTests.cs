using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Portfolio.Command;
using Portfolio.Repository;
using MongoDB.Driver;
using IntegratedTests.Fixtures;
using Infrastructure.Repository.Entities;
using Portfolio.Repository.Interface; // Para o MongoDbFixture

namespace IntegratedTests
{
    public class PortfolioControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<MongoDbFixture>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly MongoDbFixture _mongoDbFixture;

        public PortfolioControllerIntegrationTests(WebApplicationFactory<Program> factory, MongoDbFixture mongoDbFixture)
        {
            _factory = factory;
            _mongoDbFixture = mongoDbFixture;
        }

        [Fact(DisplayName = "POST /Portfolio should operate portfolio successfully")]
        public async Task OperatePortfolio_ShouldOperatePortfolioSuccessfully()
        {
            // Arrange
            var command = new OperatePortfolioCustomerCommand(Guid.NewGuid(), 1, "Product", 10, "BUY");
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IPortfolioRepository>(_ =>
                        new PortfolioRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Portfolios")); // Nome da coleção
                });
            }).CreateClient();

            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/Portfolio", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("trade realizado com sucesso", responseString);
        }

        [Fact(DisplayName = "GET /Portfolio should return all portfolios")]
        public async Task GetAll_ShouldReturnAllPortfolios()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IPortfolioRepository>(_ =>
                        new PortfolioRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Portfolios")); // Nome da coleção
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/Portfolio");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responsePortfolios = JsonConvert.DeserializeObject<List<PortfolioDomain>>(responseString);
            Assert.NotNull(responsePortfolios);
            Assert.NotEmpty(responsePortfolios);
        }

        [Fact(DisplayName = "GET /Portfolio should return portfolio by customer ID")]
        public async Task GetByCustomer_ShouldReturnPortfolioByCustomerId()
        {
            // Arrange
            var customerId = 1UL;
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IPortfolioRepository>(_ =>
                        new PortfolioRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Portfolios")); // Nome da coleção
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync($"/Portfolio?customerId={customerId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responsePortfolio = JsonConvert.DeserializeObject<PortfolioDomain>(responseString);
            Assert.NotNull(responsePortfolio);
            Assert.Equal(customerId, responsePortfolio.CustomerId);
        }

        [Fact(DisplayName = "GET /Portfolio/statement should return portfolio statement by customer ID")]
        public async Task GetStatementByCustomer_ShouldReturnPortfolioStatementByCustomerId()
        {
            // Arrange
            var customerId = 1UL;
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IPortfolioRepository>(_ =>
                        new PortfolioRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "PortfolioStatements")); // Nome da coleção de statements
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync($"/Portfolio/statement?customerId={customerId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseStatements = JsonConvert.DeserializeObject<List<PortfolioStatementDomain>>(responseString);
            Assert.NotNull(responseStatements);
            Assert.NotEmpty(responseStatements);
        }
    }
}
