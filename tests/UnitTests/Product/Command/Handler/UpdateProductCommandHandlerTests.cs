using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Repository.Entities;
using MediatR;
using Moq;
using Products.Command;
using Products.Command.Handler;
using Products.Event;
using Xunit;

namespace UnitTests.Product.Command.Handler
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UpdateProductCommandHandler _handler;
        private readonly Mock<IMapper> _mapperMock;

        public UpdateProductCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateProductCommandHandler(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact(DisplayName = "Should update product successfully")]
        public async Task Handle_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var command = new UpdateProductCommand(product.Id, product.Name, product.ProductType, (ulong)product.UnitPrice, (ulong)product.AvailableQuantity, product.ExpirationDate, product.UserId);
            var productEvent = new UpdateProductEvent();
            _mapperMock.Setup(m => m.Map<UpdateProductEvent>(command)).Returns(productEvent);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Publish(It.IsAny<UpdateProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("Produto alterado com sucesso", result);
        }

        [Fact(DisplayName = "Should throw exception when updating product")]
        public async Task Handle_Failure()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var command = new UpdateProductCommand(product.Id, product.Name, product.ProductType, (ulong)product.UnitPrice, (ulong)product.AvailableQuantity, product.ExpirationDate, product.UserId);
            _mapperMock.Setup(m => m.Map<UpdateProductEvent>(command)).Throws(new Exception("Error mapping event"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact(DisplayName = "Should throw exception when mediator publish fails")]
        public async Task Handle_MediatorPublishFailure()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var command = new UpdateProductCommand(product.Id, product.Name, product.ProductType, (ulong)product.UnitPrice, (ulong)product.AvailableQuantity, product.ExpirationDate, product.UserId);
            var productEvent = new UpdateProductEvent();
            _mapperMock.Setup(m => m.Map<UpdateProductEvent>(command)).Returns(productEvent);
            _mediatorMock.Setup(m => m.Publish(It.IsAny<UpdateProductEvent>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Error publishing event"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact(DisplayName = "Should map command to event correctly")]
        public async Task Handle_MapCommandToEvent()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var command = new UpdateProductCommand(product.Id, product.Name, product.ProductType, (ulong)product.UnitPrice, (ulong)product.AvailableQuantity, product.ExpirationDate, product.UserId);
            var productEvent = new UpdateProductEvent();
            _mapperMock.Setup(m => m.Map<UpdateProductEvent>(command)).Returns(productEvent);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map<UpdateProductEvent>(command), Times.Once);
        }

        [Fact(DisplayName = "Should not publish event if mapping fails")]
        public async Task Handle_NoPublishIfMappingFails()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var command = new UpdateProductCommand(product.Id, product.Name, product.ProductType, (ulong)product.UnitPrice, (ulong)product.AvailableQuantity, product.ExpirationDate, product.UserId);
            _mapperMock.Setup(m => m.Map<UpdateProductEvent>(command)).Throws(new Exception("Error mapping event"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            _mediatorMock.Verify(m => m.Publish(It.IsAny<UpdateProductEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
