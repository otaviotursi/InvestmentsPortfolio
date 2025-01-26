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

    public class GetPortfolioAllCustomersQueryHandlerTests
    {
        private readonly Mock<ICacheHelper> _cacheHelper;
        private readonly Mock<IPortfolioRepository> _repositoryMock;
        private readonly GetPortfolioAllCustomersQueryHandler _handler;

        public GetPortfolioAllCustomersQueryHandlerTests()
        {
            _cacheHelper = new Mock<ICacheHelper>();
            _repositoryMock = new Mock<IPortfolioRepository>();
            _handler = new GetPortfolioAllCustomersQueryHandler(_repositoryMock.Object, _cacheHelper.Object);
        }

        [Fact(DisplayName = "Handle should return list of PortfolioDomain")]
        public async Task Handle_ShouldReturnListOfPortfolioDomain()
        {
            // Arrange
            var portfolioList = new List<PortfolioDomain>
            {
                new PortfolioDomain { CustomerId = 1, ItensPortfolio = new List<ItemPortfolio>() },
                new PortfolioDomain { CustomerId = 2, ItensPortfolio = new List<ItemPortfolio>() }
            };

            _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(portfolioList);

            var query = new GetPortfolioAllCustomersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(portfolioList, result);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Repository exception"));

            var query = new GetPortfolioAllCustomersQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
