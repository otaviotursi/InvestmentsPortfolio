using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using Moq;
using Newtonsoft.Json;
using Statement.Command.Handler;
using Statement.Event;

namespace UnitTests.Statement.Event.Handler
{
    public class InsertPortfolioStatementByCustomerEventHandlerTests
    {
        private readonly Mock<IKafkaProducerService> _kafkaProducerServiceMock;
        private readonly InsertPortfolioStatementByCustomerEventHandler _handler;

        public InsertPortfolioStatementByCustomerEventHandlerTests()
        {
            _kafkaProducerServiceMock = new Mock<IKafkaProducerService>();
            _handler = new InsertPortfolioStatementByCustomerEventHandler(_kafkaProducerServiceMock.Object);
        }

        [Fact(DisplayName = "Handle should publish message to Kafka")]
        public async Task Handle_ShouldPublishMessageToKafka()
        {
            // Arrange
            var productEvent = new InsertPortfolioStatementByCustomerEvent
            {
                CustomerId = 1,
                ProductId = Guid.NewGuid(),
                ProductName = "Product",
                AmountNegotiated = 10,
                OperationType = "BUY",
                TransactionDate = DateTime.UtcNow
            };

            _kafkaProducerServiceMock.Setup(k => k.PublishMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(productEvent, CancellationToken.None);

            // Assert
            _kafkaProducerServiceMock.Verify(k => k.PublishMessageAsync(
                KafkaTopics.InsertCustomerPorftolioStatement,
                productEvent.CustomerId.ToString(),
                JsonConvert.SerializeObject(productEvent)), Times.Once);
        }
    }
}
