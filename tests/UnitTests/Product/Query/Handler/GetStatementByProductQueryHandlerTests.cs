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
using Products.Query.Handler;
using Products.Query;

namespace UnitTests.Product.Query.Handler
{
    public class GetStatementByProductQueryHandlerTests
    {
        private readonly Mock<IProductStatementRepository> _repositoryMock;
        private readonly GetStatementByProductQueryHandler _handler;

        public GetStatementByProductQueryHandlerTests()
        {
            _repositoryMock = new Mock<IProductStatementRepository>();
            _handler = new GetStatementByProductQueryHandler(Mock.Of<IMediator>(), _repositoryMock.Object);
        }

        [Fact(DisplayName = "Should get statement by product successfully")]
        public async Task Handle_Success()
        {
            string? productName = "TestProduct";
            ulong? userId = 12345;
            DateTime? expirationDate = DateTime.UtcNow.AddDays(30);
            Guid? productId = Guid.NewGuid();
            // Arrange
            var products = new List<ProductDomain>
            {
                new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1),
                new ProductDomain(Guid.NewGuid(), "Product2", 200, 20, "Type2", DateTime.UtcNow, 2)
            };
            _repositoryMock.Setup(r => r.GetStatementBy(productName, userId, expirationDate, productId, CancellationToken.None)).ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(new GetStatementByProductQuery(productName, userId, expirationDate, productId), CancellationToken.None);

            // Assert
            Assert.Equal(products, result);
        }

        [Fact(DisplayName = "Should throw exception when repository get statement by product fails")]
        public async Task Handle_Failure()
        {
            string? productName = "TestProduct";
            ulong? userId = 12345;
            DateTime? expirationDate = DateTime.UtcNow.AddDays(30);
            Guid? productId = Guid.NewGuid();

            // Arrange
            _repositoryMock.Setup(r => r.GetStatementBy(productName, userId, expirationDate, productId, CancellationToken.None)).ThrowsAsync(new Exception("Repository get statement by product failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetStatementByProductQuery(productName, userId, expirationDate, productId), CancellationToken.None));
        }
    }
}

