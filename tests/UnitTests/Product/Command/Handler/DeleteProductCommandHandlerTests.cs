using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using Products.Command;
using Products.Command.Handler;
using Products.Event;
using Xunit;

namespace UnitTests.Product.Command.Handler
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DeleteProductCommandHandler _handler;
        private readonly Mock<IMapper> _mapperMock;

        public DeleteProductCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _handler = new DeleteProductCommandHandler(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Should delete product successfully")]
        public async Task Handle_Success()
        {
            // Arrange
            var command = new DeleteProductCommand(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Publish(It.IsAny<DeleteProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("Produto excluido com sucesso", result);
        }

        [Fact(DisplayName = "Should throw exception when updating product")]
        public async Task Handle_Failure()
        {
            // Arrange
            var command = new DeleteProductCommand(Guid.NewGuid());
            _mediatorMock.Setup(m => m.Publish(It.IsAny<DeleteProductEvent>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Erro ao excluir produto"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
