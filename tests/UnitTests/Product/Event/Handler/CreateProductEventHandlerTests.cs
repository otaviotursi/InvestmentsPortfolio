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
using Infrastructure.Repository.Entities;

namespace UnitTests.Product.Event.Handler
{
    public class CreateProductEventHandlerTests
    {
        private readonly Mock<IKafkaProducerService> _kafkaProducerServiceMock;
        private readonly Mock<IProductStatementRepository> _repositoryWriteMock;
        private readonly CreateProductEventHandler _handler;

        public CreateProductEventHandlerTests()
        {
            _kafkaProducerServiceMock = new Mock<IKafkaProducerService>();
            _repositoryWriteMock = new Mock<IProductStatementRepository>();
            _handler = new CreateProductEventHandler(_kafkaProducerServiceMock.Object, _repositoryWriteMock.Object);
        }

        [Fact(DisplayName = "Should handle create product event successfully")]
        public async Task Handle_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var productEvent = new CreateProductEvent(product);

            // Act
            await _handler.Handle(productEvent, CancellationToken.None);

            // Assert
            _repositoryWriteMock.Verify(r => r.InsertAsync(It.IsAny<ProductDomain>(), It.IsAny<CancellationToken>()), Times.Once);
            _kafkaProducerServiceMock.Verify(k => k.PublishMessageAsync(KafkaTopics.InsertProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent)), Times.Once);
        }

        [Fact(DisplayName = "Should throw exception when repository insert fails")]
        public async Task Handle_RepositoryInsertFailure()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var productEvent = new CreateProductEvent(product);
            _repositoryWriteMock.Setup(r => r.InsertAsync(It.IsAny<ProductDomain>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository insert failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(productEvent, CancellationToken.None));
        }

        [Fact(DisplayName = "Should throw exception when Kafka publish fails")]
        public async Task Handle_KafkaPublishFailure()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "ProductName", 100, 10, "ProductType", DateTime.UtcNow, 1);
            var productEvent = new CreateProductEvent(product);
            _kafkaProducerServiceMock.Setup(k => k.PublishMessageAsync(KafkaTopics.InsertProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent))).ThrowsAsync(new Exception("Kafka publish failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(productEvent, CancellationToken.None));
        }
    }
}
