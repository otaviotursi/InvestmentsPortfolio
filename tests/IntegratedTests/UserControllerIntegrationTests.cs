using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Users.Command;
using Infrastructure.Repository.Entities;
using MongoDB.Driver;
using IntegratedTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Users.Repository.Interface;
using Users.Repository; // Para o MongoDbFixture

namespace IntegratedTests
{
    public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<MongoDbFixture>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly MongoDbFixture _mongoDbFixture;

        public UserControllerIntegrationTests(WebApplicationFactory<Program> factory, MongoDbFixture mongoDbFixture)
        {
            _factory = factory;
            _mongoDbFixture = mongoDbFixture;
        }

        [Fact(DisplayName = "POST /User should create user successfully")]
        public async Task CreateUser_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var command = new CreateUserCommand("John Doe", "johndoe");
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IUserRepository>(_ =>
                        new UserRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Users")); // Nome da coleção
                });
            }).CreateClient();

            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/User", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Usuário criado com sucesso", responseString);
        }

        [Fact(DisplayName = "PUT /User should update user successfully")]
        public async Task UpdateUser_ShouldUpdateUserSuccessfully()
        {
            // Arrange
            var command = new UpdateUserCommand(1UL, "John Doe", "johndoe");
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IUserRepository>(_ =>
                        new UserRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Users")); // Nome da coleção
                });
            }).CreateClient();

            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync("/User", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Usuário 1 alterado com sucesso", responseString);
        }

        [Fact(DisplayName = "GET /User should return all users")]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IUserRepository>(_ =>
                        new UserRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Users")); // Nome da coleção
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/User");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseUsers = JsonConvert.DeserializeObject<List<UserDomain>>(responseString);
            Assert.NotNull(responseUsers);
            Assert.NotEmpty(responseUsers);
        }

        [Fact(DisplayName = "GET /User should return user by query")]
        public async Task GetByUser_ShouldReturnUserByQuery()
        {
            // Arrange
            var userId = 1UL;
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IUserRepository>(_ =>
                        new UserRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Users")); // Nome da coleção
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync($"/User?id={userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseUser = JsonConvert.DeserializeObject<UserDomain>(responseString);
            Assert.NotNull(responseUser);
            Assert.Equal(userId, responseUser.Id);
        }

        [Fact(DisplayName = "DELETE /User/{id} should delete user successfully")]
        public async Task Delete_ShouldDeleteUserSuccessfully()
        {
            // Arrange
            var userId = 1UL;
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Registrar MongoDB
                    services.AddSingleton<IMongoClient>(_ => _mongoDbFixture.Client);
                    services.AddScoped<IUserRepository>(_ =>
                        new UserRepository(
                            _mongoDbFixture.Client,
                            _mongoDbFixture.DatabaseName,
                            "Users")); // Nome da coleção
                });
            }).CreateClient();

            // Act
            var response = await client.DeleteAsync($"/User/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("Usuário excluído com sucesso", responseString);
        }
    }
}
