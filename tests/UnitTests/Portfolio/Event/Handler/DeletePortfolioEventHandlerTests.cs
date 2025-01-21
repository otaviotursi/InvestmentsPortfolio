using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using Moq;
using Newtonsoft.Json;
using Portfolio.Event;
using Portfolio.Event.Handler;

namespace UnitTests.Portfolio.Event.Handler
{

    public class DeletePortfolioEventHandlerTests
    {
        private readonly Mock<IKafkaProducerService> _kafkaProducerServiceMock;
        private readonly DeletePortfolioEventHandler _handler;

        public DeletePortfolioEventHandlerTests()
        {
            _kafkaProducerServiceMock = new Mock<IKafkaProducerService>();
            _handler = new DeletePortfolioEventHandler(_kafkaProducerServiceMock.Object);
        }

        [Fact(DisplayName = "Handle should publish message to Kafka")]
        public async Task Handle_ShouldPublishMessageToKafka()
        {
            // Arrange
            var portfolioEvent = new DeletePortfolioEvent
            {
                CustomerId = 1,
                ProductId = Guid.NewGuid(),
                ProductName = "Product",
                AmountNegotiated = 10,
                ValueNegotiated = 100m,
                OperationType = "SELL"
            };

            _kafkaProducerServiceMock.Setup(k => k.PublishMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(portfolioEvent, CancellationToken.None);

            // Assert
            _kafkaProducerServiceMock.Verify(k => k.PublishMessageAsync(
                KafkaTopics.InsertCustomerPorftolio,
                portfolioEvent.CustomerId + " - " + portfolioEvent.ProductId,
                JsonConvert.SerializeObject(portfolioEvent)), Times.Once);
        }

        [Fact(DisplayName = "Handle should handle cancellation token")]
        public async Task Handle_ShouldHandleCancellationToken()
        {
            // Arrange
            var portfolioEvent = new DeletePortfolioEvent
            {
                CustomerId = 1,
                ProductId = Guid.NewGuid(),
                ProductName = "Product",
                AmountNegotiated = 10,
                ValueNegotiated = 100m,
                OperationType = "SELL"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            _kafkaProducerServiceMock.Setup(k => k.PublishMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(portfolioEvent, cancellationToken);

            // Assert
            _kafkaProducerServiceMock.Verify(k => k.PublishMessageAsync(
                KafkaTopics.InsertCustomerPorftolio,
                portfolioEvent.CustomerId + " - " + portfolioEvent.ProductId,
                JsonConvert.SerializeObject(portfolioEvent)), Times.Once);
        }
    }
}
