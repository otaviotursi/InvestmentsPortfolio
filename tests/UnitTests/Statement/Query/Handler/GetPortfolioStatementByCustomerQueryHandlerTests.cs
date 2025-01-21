using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Repository.Entities;
using MediatR;
using Moq;
using Statement.Command;
using Statement.Command.Handler;
using Statement.Repository.Interface;

namespace UnitTests.Statement.Query.Handler
{

    public class GetPortfolioStatementByCustomerQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IPortfolioStatementRepository> _repositoryMock;
        private readonly GetPortfolioStatementByCustomerQueryHandler _handler;

        public GetPortfolioStatementByCustomerQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<IPortfolioStatementRepository>();
            _handler = new GetPortfolioStatementByCustomerQueryHandler(_mediatorMock.Object, _repositoryMock.Object);
        }

        [Fact(DisplayName = "Handle should return list of PortfolioStatementDomain for given customer ID")]
        public async Task Handle_ShouldReturnListOfPortfolioStatementDomain_ForGivenCustomerId()
        {
            // Arrange
            var customerId = 1UL;
            var portfolioStatements = new List<PortfolioStatementDomain>
        {
            new PortfolioStatementDomain { CustomerId = customerId, ProductId = Guid.NewGuid(), ProductName = "Product1", AmountNegotiated = 10, OperationType = "BUY", TransactionDate = DateTime.UtcNow },
            new PortfolioStatementDomain { CustomerId = customerId, ProductId = Guid.NewGuid(), ProductName = "Product2", AmountNegotiated = 5, OperationType = "SELL", TransactionDate = DateTime.UtcNow }
        };

            _repositoryMock.Setup(r => r.GetByCustomerId(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(portfolioStatements);

            var query = new GetPortfolioStatementByCustomerQuery(customerId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(portfolioStatements, result);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var customerId = 1UL;

            _repositoryMock.Setup(r => r.GetByCustomerId(customerId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Repository exception"));

            var query = new GetPortfolioStatementByCustomerQuery(customerId);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
