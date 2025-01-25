using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Cache;
using Infrastructure.Repository.Entities;
using MediatR;
using Moq;
using Portfolio.Command;
using Portfolio.Command.Handler;
using Portfolio.Repository.Interface;

namespace UnitTests.Portfolio.Query.Handler
{

    public class GetPortfolioByCustomerQueryHandlerTests
    {
        private readonly Mock<ICacheHelper> _cacheHelper;
        private readonly Mock<IPortfolioRepository> _repositoryMock;
        private readonly GetPortfolioByCustomerQueryHandler _handler;

        public GetPortfolioByCustomerQueryHandlerTests()
        {
            _cacheHelper = new Mock<ICacheHelper>();
            _repositoryMock = new Mock<IPortfolioRepository>();
            _handler = new GetPortfolioByCustomerQueryHandler(_repositoryMock.Object, _cacheHelper.Object);
        }

        [Fact(DisplayName = "Handle should return PortfolioDomain for given customer ID")]
        public async Task Handle_ShouldReturnPortfolioDomain_ForGivenCustomerId()
        {
            // Arrange
            var customerId = 1UL;
            var portfolio = new PortfolioDomain { CustomerId = customerId, ItensPortfolio = new List<ItemPortfolio>() };

            _repositoryMock.Setup(r => r.GetById(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(portfolio);

            var query = new GetPortfolioByCustomerQuery(customerId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(portfolio, result);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var customerId = 1UL;

            _repositoryMock.Setup(r => r.GetById(customerId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Repository exception"));

            var query = new GetPortfolioByCustomerQuery(customerId);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
