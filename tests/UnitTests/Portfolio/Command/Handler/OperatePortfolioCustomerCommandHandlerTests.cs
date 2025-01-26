

using AutoMapper;
using Infrastructure.Repository.Entities;
using MediatR;
using Moq;
using Portfolio.Command;
using Portfolio.Command.Handler;
using Portfolio.Event;
using Portfolio.Repository.Interface;
using Products.Command;
using Products.Event;
using Statement.Event;

namespace UnitTests.Portfolio.Command.Handler
{

    public class OperatePortfolioCustomerCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OperatePortfolioCustomerCommandHandler _handler;

        public OperatePortfolioCustomerCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _handler = new OperatePortfolioCustomerCommandHandler(_mediatorMock.Object,  _mapperMock.Object);
        }

        //[Fact(DisplayName = "Handle should throw exception when available quantity is insufficient for buy operation")]
        //public async Task Handle_ShouldThrowException_WhenAvailableQuantityIsInsufficientForBuyOperation()
        //{
        //    // Arrange
        //    var command = new OperatePortfolioCustomerCommand(Guid.NewGuid(), 1, "Product", 10, "BUY");
        //    var productQueryResult = new ProductDomain { AvailableQuantity = 5, UnitPrice = 100m, ProductType = "Type", Name = "Product" };

        //    _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByQuery>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(productQueryResult);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        //}

        [Fact(DisplayName = "Handle should publish InsertPortfolioEvent and UpdateProductEvent for buy operation")]
        public async Task Handle_ShouldPublishInsertPortfolioEventAndUpdateProductEvent_ForBuyOperation()
        {
            // Arrange
            var command = new OperatePortfolioCustomerCommand(Guid.NewGuid(), 1, "Product", 10, "BUY");
            var productQueryResult = new ProductDomain { AvailableQuantity = 15, UnitPrice = 100m, ProductType = "Type", Name = "Product" };
            var portfolioRequest = new PortfolioRequest();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productQueryResult);
            _mapperMock.Setup(m => m.Map<PortfolioRequest>(command)).Returns(portfolioRequest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Publish(It.IsAny<InsertPortfolioEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<UpdateProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("trade realizado com sucesso", result);
        }

        [Fact(DisplayName = "Handle should publish DeletePortfolioEvent and UpdateProductEvent for sell operation")]
        public async Task Handle_ShouldPublishDeletePortfolioEventAndUpdateProductEvent_ForSellOperation()
        {
            // Arrange
            var command = new OperatePortfolioCustomerCommand(Guid.NewGuid(), 1, "Product", 10, "SELL");
            var productQueryResult = new ProductDomain { AvailableQuantity = 15, UnitPrice = 100m, ProductType = "Type", Name = "Product" };
            var portfolioRequest = new PortfolioRequest();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productQueryResult);
            _mapperMock.Setup(m => m.Map<PortfolioRequest>(command)).Returns(portfolioRequest);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Publish(It.IsAny<DeletePortfolioEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<UpdateProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("trade realizado com sucesso", result);
        }

        [Fact(DisplayName = "Handle should publish InsertPortfolioStatementByCustomerEvent")]
        public async Task Handle_ShouldPublishInsertPortfolioStatementByCustomerEvent()
        {
            // Arrange
            var command = new OperatePortfolioCustomerCommand(Guid.NewGuid(), 1, "Product", 10, "BUY");
            var productQueryResult = new ProductDomain { AvailableQuantity = 15, UnitPrice = 100m, ProductType = "Type", Name = "Product" };
            var portfolioRequest = new PortfolioRequest();
            var portfolioStatementEvent = new InsertPortfolioStatementByCustomerEvent();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(productQueryResult);
            _mapperMock.Setup(m => m.Map<PortfolioRequest>(command)).Returns(portfolioRequest);
            _mapperMock.Setup(m => m.Map<InsertPortfolioStatementByCustomerEvent>(command)).Returns(portfolioStatementEvent);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Publish(It.IsAny<InsertPortfolioStatementByCustomerEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal("trade realizado com sucesso", result);
        }
    }
}
