using System.Text;
using MediatR;
using Moq;
using Newtonsoft.Json;
using Portfolio.Command;
using Statement.Command;
using Infrastructure.Repository.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;


namespace FunctionalTests
{

    public class PortfolioControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IMediator> _mediatorMock;

    public PortfolioControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mediatorMock = new Mock<IMediator>();
    }

    [Fact(DisplayName = "POST /Portfolio should return success response")]
    public async Task OperatePortfolio_ShouldReturnSuccessResponse()
    {
        // Arrange
        var command = new OperatePortfolioCustomerCommand(Guid.NewGuid(), 1, "Product", 10, "BUY");
        _mediatorMock.Setup(m => m.Send(It.IsAny<OperatePortfolioCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("trade realizado com sucesso");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
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

        //[Fact(DisplayName = "GET /Portfolio should return all customers")]
        //public async Task Get_ShouldReturnAllCustomers()
        //{
        //    // Arrange
        //    var customers = new List<CustomerDomain>
        //    {
        //        new CustomerDomain { Id = 1, FullName = "John Doe", User = "johndoe" },
        //        new CustomerDomain { Id = 2, FullName = "Jane Doe", User = "janedoe" }
        //    };

        //    _mediatorMock.Setup(m => m.Send(It.IsAny<GetPortfolioAllCustomersQuery>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(customers);

        //    var client = _factory.WithWebHostBuilder(builder =>
        //    {
        //        builder.ConfigureServices(services =>
        //        {
        //            services.AddSingleton(_mediatorMock.Object);
        //        });
        //    }).CreateClient();

        //    // Act
        //    var response = await client.GetAsync("/Portfolio");

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var responseCustomers = JsonConvert.DeserializeObject<List<CustomerDomain>>(responseString);
        //    Assert.Equal(customers, responseCustomers);
        //}

        [Fact(DisplayName = "GET /Portfolio/statement should return customer statement")]
    public async Task GetStatement_ShouldReturnCustomerStatement()
    {
        // Arrange
        var customerId = 1UL;
        var statements = new List<PortfolioStatementDomain>
        {
            new PortfolioStatementDomain { CustomerId = customerId, ProductId = Guid.NewGuid(), ProductName = "Product1", AmountNegotiated = 10, OperationType = "BUY", TransactionDate = DateTime.UtcNow },
            new PortfolioStatementDomain { CustomerId = customerId, ProductId = Guid.NewGuid(), ProductName = "Product2", AmountNegotiated = 5, OperationType = "SELL", TransactionDate = DateTime.UtcNow }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetPortfolioStatementByCustomerQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statements);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mediatorMock.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync($"/Portfolio/statement?customerId={customerId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var responseStatements = JsonConvert.DeserializeObject<List<PortfolioStatementDomain>>(responseString);
        Assert.Equal(statements.ToString(), responseStatements.ToString());
    }
}
}
