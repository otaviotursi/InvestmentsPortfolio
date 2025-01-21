using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Customers.Command;
using Customers.Repository;
using MongoDB.Driver;
using IntegratedTests.Fixtures;
using Customers.Repository.Interface; // Supondo que você tenha uma fixture para o Mongo2Go

namespace IntegratedTests
{
    public class CustomerControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<MongoDbFixture>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly MongoDbFixture _mongoDbFixture;

        public CustomerControllerIntegrationTests(WebApplicationFactory<Program> factory, MongoDbFixture mongoDbFixture)
        {
            _factory = factory;
            _mongoDbFixture = mongoDbFixture;
        }

        [Fact(DisplayName = "POST /Customer should create customer successfully")]
        public async Task CreateCustomer_ShouldCreateCustomerSuccessfully()
        {
            // Arrange
            var command = new CreateCustomerCommand("John Doe", "johndoe");
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<ICustomerRepository>(_ =>
                        new CustomerRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Customers")); // Nome da coleção
                });
            }).CreateClient();

            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/Customer", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Cliente criado com sucesso", responseString);
        }
    }
}
