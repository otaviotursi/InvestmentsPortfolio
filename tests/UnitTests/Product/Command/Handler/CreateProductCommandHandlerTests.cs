
using AutoMapper;
using Infrastructure.Repository.Entities;
using MediatR;
using Moq;
using Products.Command;
using Products.Command.Handler;
using Products.Event;

namespace UnitTests.Product.Command.Handler
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateProductCommandHandler(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact(DisplayName = "Handle should return success message when product is created successfully")]
        public async Task Handle_ShouldReturnSuccessMessage_WhenProductIsCreatedSuccessfully()
        {
            // Arrange
            var command = new CreateProductCommand();
            var productDomain = new ProductDomain();
            var productEvent = new CreateProductEvent();

            _mapperMock.Setup(m => m.Map<ProductDomain>(command)).Returns(productDomain);
            _mapperMock.Setup(m => m.Map<CreateProductEvent>(command)).Returns(productEvent);
            _mediatorMock.Setup(m => m.Publish(It.IsAny<CreateProductEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Produto criado com sucesso", result);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<CreateProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw exception when an error occurs")]
        public async Task Handle_ShouldThrowException_WhenAnErrorOccurs()
        {
            // Arrange
            var command = new CreateProductCommand();
            _mapperMock.Setup(m => m.Map<ProductDomain>(command)).Throws(new Exception("Mapping error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
