using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Products.Repository.Interface;
using Products.Service.Kafka;
using Xunit;
using Infrastructure.Repository.Entities;
using Infrastructure.Kafka;
using Infrastructure.Email;
using Infrastructure.Email.Interface;
using Investments.Infrastructure.Kafka;

namespace UnitTests.Product.Services.Kafka
{
    public class ProductKafkaConsumerServiceTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IEmailNotificationService> _emailNotificationServiceMock;
        private readonly Mock<ILogger<ProductKafkaConsumerService>> _loggerMock;
        private readonly Mock<IConsumer<string, string>> _consumerMock;
        private readonly ProductKafkaConsumerService _service;
        private readonly KafkaConfig _kafkaConfig;
        private readonly EmailConfig _emailConfig;

        public ProductKafkaConsumerServiceTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _emailNotificationServiceMock = new Mock<IEmailNotificationService>();
            _loggerMock = new Mock<ILogger<ProductKafkaConsumerService>>();
            _consumerMock = new Mock<IConsumer<string, string>>();

            _kafkaConfig = new KafkaConfig
            {
                BootstrapServers = "localhost:9092",
                ConsumerGroupId = "test-group"
            };

            _emailConfig = new EmailConfig
            {
                EmailSendExpiration = "test@example.com"
            };

            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IProductRepository))).Returns(_productRepositoryMock.Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IEmailNotificationService))).Returns(_emailNotificationServiceMock.Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IServiceScopeFactory))).Returns(_serviceScopeFactoryMock.Object);

            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

            _service = new ProductKafkaConsumerService(
                _serviceProviderMock.Object,
                Options.Create(_kafkaConfig),
                _loggerMock.Object,
                Options.Create(_emailConfig)
            );

            _service.GetType().GetField("_consumer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_service, _consumerMock.Object);
        }

        [Fact(DisplayName = "Should process insert product message successfully")]
        public async Task ProcessMessageAsync_InsertProduct_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
            var message = JsonConvert.SerializeObject(product);
            _consumerMock.Setup(c => c.Consume(It.IsAny<TimeSpan>())).Returns(new ConsumeResult<string, string>
            {
                Topic = KafkaTopics.InsertProductTopic,
                Message = new Message<string, string> { Key = product.Id.ToString(), Value = message }
            });

            // Act
            await _service.StartAsync(CancellationToken.None);
            await Task.Delay(1000); // Aguarde um pouco para o processamento da mensagem

            // Assert
            _productRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<ProductDomain>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Should process update product message successfully")]
        public async Task ProcessMessageAsync_UpdateProduct_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
            var message = JsonConvert.SerializeObject(product);
            _consumerMock.Setup(c => c.Consume(It.IsAny<TimeSpan>())).Returns(new ConsumeResult<string, string>
            {
                Topic = KafkaTopics.UpdateProductTopic,
                Message = new Message<string, string> { Key = product.Id.ToString(), Value = message }
            });

            // Act
            await _service.StartAsync(CancellationToken.None);
            await Task.Delay(1000); // Aguarde um pouco para o processamento da mensagem

            // Assert
            _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ProductDomain>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Should process delete product message successfully")]
        public async Task ProcessMessageAsync_DeleteProduct_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
            var message = JsonConvert.SerializeObject(product);
            _consumerMock.Setup(c => c.Consume(It.IsAny<TimeSpan>())).Returns(new ConsumeResult<string, string>
            {
                Topic = KafkaTopics.DeleteProductTopic,
                Message = new Message<string, string> { Key = product.Id.ToString(), Value = message }
            });

            // Act
            await _service.StartAsync(CancellationToken.None);
            await Task.Delay(1000); // Aguarde um pouco para o processamento da mensagem

            // Assert
            _productRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Should process product expiry notification message successfully")]
        public async Task ProcessMessageAsync_ProductExpiryNotification_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow.AddDays(5), 1);
            var products = new List<ProductDomain> { product };
            _productRepositoryMock.Setup(r => r.GetExpiritionByDateAll(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(products);
            _consumerMock.Setup(c => c.Consume(It.IsAny<TimeSpan>())).Returns(new ConsumeResult<string, string>
            {
                Topic = KafkaTopics.ProductExpiryNotificationTopic,
                Message = new Message<string, string> { Key = product.Id.ToString(), Value = string.Empty }
            });

            // Act
            await _service.StartAsync(CancellationToken.None);
            await Task.Delay(1000); // Aguarde um pouco para o processamento da mensagem

            // Assert
            _emailNotificationServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<EmailRequest>()), Times.Once);
        }
    }
}

