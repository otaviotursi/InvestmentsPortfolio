using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Products.Command.Handler;
using Products.Repository.Interface;
using Xunit;
using MediatR;
using Infrastructure.Repository.Entities;
using Products.Command;
using Infrastructure.Cache;

namespace UnitTests.Product.Query.Handler
{
    public class GetProductByQueryHandlerTests
    {
        private readonly Mock<ICacheHelper> _cacheHelper;
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly GetProductByQueryHandler _handler;

        public GetProductByQueryHandlerTests()
        {
            _cacheHelper = new Mock<ICacheHelper>();
            _repositoryMock = new Mock<IProductRepository>();
            _handler = new GetProductByQueryHandler( _repositoryMock.Object, _cacheHelper.Object);
        }

        [Fact(DisplayName = "Should get product by id successfully")]
        public async Task Handle_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
            _repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(product);

            // Act
            var result = await _handler.Handle(new GetProductByQuery(product.Id), CancellationToken.None);

            // Assert
            Assert.Equal(product, result);
        }

        [Fact(DisplayName = "Should throw exception when repository get by id fails")]
        public async Task Handle_Failure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository get by id failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetProductByQuery(Guid.NewGuid()), CancellationToken.None));
        }
    }
}

