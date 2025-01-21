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
using Customers.Command;
using Infrastructure.Repository.Entities;
using Microsoft.AspNetCore.Mvc.Testing;



namespace FunctionalTests
{

    public class CustomerControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMediator> _mediatorMock;

    public CustomerControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact(DisplayName = "POST /Customer should create customer successfully")]
    public async Task CreateCustomer_ShouldCreateCustomerSuccessfully()
    {
        // Arrange
        var command = new CreateCustomerCommand("John Doe", "johndoe");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Cliente criado com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
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

    [Fact(DisplayName = "PUT /Customer should update customer successfully")]
    public async Task UpdateCustomer_ShouldUpdateCustomerSuccessfully()
    {
        // Arrange
        var command = new UpdateCustomerCommand(1UL, "John Doe", "johndoe");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Cliente 1 alterado com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/Customer", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Cliente 1 alterado com sucesso", responseString);
    }

    [Fact(DisplayName = "GET /Customer should return all customers")]
    public async Task GetAll_ShouldReturnAllCustomers()
    {
        // Arrange
        var customers = new List<CustomerDomain>
        {
            new CustomerDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" },
            new CustomerDomain { Id = 2UL, FullName = "Jane Doe", User = "janedoe" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllCustomerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/Customer");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseCustomers = JsonConvert.DeserializeObject<List<CustomerDomain>>(responseString);
        Assert.Equal(customers.ToString(), responseCustomers.ToString());
    }

    [Fact(DisplayName = "GET /Customer should return customer by query")]
    public async Task GetByCustomer_ShouldReturnCustomerByQuery()
    {
        // Arrange
        var customerId = 1UL;
        var customer = new CustomerDomain { Id = customerId, FullName = "John Doe", User = "johndoe" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetByCustomerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync($"/Customer?id={customerId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseCustomer = JsonConvert.DeserializeObject<CustomerDomain>(responseString);
        Assert.Equal(customer.ToString(), responseCustomer.ToString());
    }

    [Fact(DisplayName = "DELETE /Customer/{id} should delete customer successfully")]
    public async Task Delete_ShouldDeleteCustomerSuccessfully()
    {
        // Arrange
        var customerId = 1UL;
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Cliente excluido com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.DeleteAsync($"/Customer/{customerId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("Cliente excluido com sucesso", responseString);
    }
}
}


