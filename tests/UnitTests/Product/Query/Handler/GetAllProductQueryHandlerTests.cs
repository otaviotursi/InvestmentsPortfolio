using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Products.Command.Handler;
using Products.Repository.Interface;
using Xunit;
using MediatR;
using Infrastructure.Repository.Entities;
using Products.Command;

namespace UnitTests.Product.Query.Handler
{
    public class GetAllProductQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly GetAllProductQueryHandler _handler;

        public GetAllProductQueryHandlerTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _handler = new GetAllProductQueryHandler(Mock.Of<IMediator>(), _repositoryMock.Object);
        }

        [Fact(DisplayName = "Should get all products successfully")]
        public async Task Handle_Success()
        {
            // Arrange
            var products = new List<ProductDomain>
            {
                new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1),
                new ProductDomain(Guid.NewGuid(), "Product2", 200, 20, "Type2", DateTime.UtcNow, 2)
            };
            _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>())).ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(new GetAllProductQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(products, result);
        }

        [Fact(DisplayName = "Should throw exception when repository get all fails")]
        public async Task Handle_Failure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository get all failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetAllProductQuery(), CancellationToken.None));
        }
    }
}

