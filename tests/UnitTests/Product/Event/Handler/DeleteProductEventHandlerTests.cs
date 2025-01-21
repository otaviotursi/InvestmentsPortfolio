using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Products.Event;
using Products.Event.Handler;
using Products.Repository.Interface;
using Xunit;
using Infrastructure.Services;
using Investments.Infrastructure.Kafka;

namespace UnitTests.Product.Event.Handler
{
    public class DeleteProductEventHandlerTests
    {
        private readonly Mock<IKafkaProducerService> _kafkaProducerServiceMock;
        private readonly Mock<IProductStatementRepository> _repositoryWriteMock;
        private readonly DeleteProductEventHandler _handler;

        public DeleteProductEventHandlerTests()
        {
            _kafkaProducerServiceMock = new Mock<IKafkaProducerService>();
            _repositoryWriteMock = new Mock<IProductStatementRepository>();
            _handler = new DeleteProductEventHandler(_kafkaProducerServiceMock.Object, _repositoryWriteMock.Object);
        }

        [Fact(DisplayName = "Should handle delete product event successfully")]
        public async Task Handle_Success()
        {
            // Arrange
            var productEvent = new DeleteProductEvent(Guid.NewGuid());

            // Act
            await _handler.Handle(productEvent, CancellationToken.None);

            // Assert
            _repositoryWriteMock.Verify(r => r.DeleteAsync(productEvent.Id, It.IsAny<CancellationToken>()), Times.Once);
            _kafkaProducerServiceMock.Verify(k => k.PublishMessageAsync(KafkaTopics.DeleteProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent)), Times.Once);
        }

        [Fact(DisplayName = "Should throw exception when repository delete fails")]
        public async Task Handle_RepositoryDeleteFailure()
        {
            // Arrange
            var productEvent = new DeleteProductEvent(Guid.NewGuid());
            _repositoryWriteMock.Setup(r => r.DeleteAsync(productEvent.Id, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository delete failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(productEvent, CancellationToken.None));
        }

        [Fact(DisplayName = "Should throw exception when Kafka publish fails")]
        public async Task Handle_KafkaPublishFailure()
        {
            // Arrange
            var productEvent = new DeleteProductEvent(Guid.NewGuid());
            _kafkaProducerServiceMock.Setup(k => k.PublishMessageAsync(KafkaTopics.DeleteProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent))).ThrowsAsync(new Exception("Kafka publish failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(productEvent, CancellationToken.None));
        }
    }
}
