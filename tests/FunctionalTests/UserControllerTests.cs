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
using Users.Command;
using Users.Query;
using Infrastructure.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Testing;



namespace FunctionalTests
{

    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMediator> _mediatorMock;

    public UserControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact(DisplayName = "POST /User should create user successfully")]
    public async Task CreateUser_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var command = new CreateUserCommand("John Doe", "johndoe");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Usuário criado com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
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
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Usuário 1 alterado com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
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
        var users = new List<UserDomain>
        {
            new UserDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" },
            new UserDomain { Id = 2UL, FullName = "Jane Doe", User = "janedoe" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/User");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseUsers = JsonConvert.DeserializeObject<List<UserDomain>>(responseString);
        Assert.Equal(users.ToString(), responseUsers.ToString());
    }

    [Fact(DisplayName = "GET /User should return user by query")]
    public async Task GetByUser_ShouldReturnUserByQuery()
    {
        // Arrange
        var userId = 1UL;
        var user = new UserDomain { Id = userId, FullName = "John Doe", User = "johndoe" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetByUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync($"/User?id={userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseUser = JsonConvert.DeserializeObject<UserDomain>(responseString);
        Assert.Equal(user.ToString(), responseUser.ToString());
    }

    [Fact(DisplayName = "DELETE /User/{id} should delete user successfully")]
    public async Task Delete_ShouldDeleteUserSuccessfully()
    {
        // Arrange
        var userId = 1UL;
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Usuário excluído com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
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