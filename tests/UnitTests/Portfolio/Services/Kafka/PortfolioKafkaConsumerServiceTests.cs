using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Infrastructure.Email;
using Infrastructure.Email.Interface;
using Infrastructure.Kafka;
using Infrastructure.Repository.Entities;
using Investments.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Portfolio.Repository.Interface;
using Portfolio.Service.Kafka;

namespace UnitTests.Portfolio.Services.Kafka
{

    public class PortfolioKafkaConsumerServiceTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IOptions<KafkaConfig>> _kafkaConfigMock;
        private readonly Mock<IOptions<EmailConfig>> _emailConfigMock;
        private readonly Mock<ILogger<PortfolioKafkaConsumerService>> _loggerMock;
        private readonly Mock<IConsumer<string, string>> _consumerMock;
        private readonly Mock<IPortfolioRepository> _repositoryMock;
        private readonly Mock<IEmailNotificationService> _emailNotificationServiceMock;
        private readonly PortfolioKafkaConsumerService _service;

        public PortfolioKafkaConsumerServiceTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _kafkaConfigMock = new Mock<IOptions<KafkaConfig>>();
            _emailConfigMock = new Mock<IOptions<EmailConfig>>();
            _loggerMock = new Mock<ILogger<PortfolioKafkaConsumerService>>();
            _consumerMock = new Mock<IConsumer<string, string>>();
            _repositoryMock = new Mock<IPortfolioRepository>();
            _emailNotificationServiceMock = new Mock<IEmailNotificationService>();

            var kafkaConfig = new KafkaConfig
            {
                BootstrapServers = "localhost:9092",
                ConsumerGroupId = "test-group"
            };
            var emailConfig = new EmailConfig
            {
                EmailSendExpiration = "7",
                Mail = "test@example.com",
                DisplayName = "Test",
                Password = "password",
                Host = "smtp.example.com",
                Port = 587
            };

            _kafkaConfigMock.Setup(x => x.Value).Returns(kafkaConfig);
            _emailConfigMock.Setup(x => x.Value).Returns(emailConfig);

            _serviceProviderMock.Setup(x => x.GetService(typeof(IPortfolioRepository))).Returns(_repositoryMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IEmailNotificationService))).Returns(_emailNotificationServiceMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(_serviceScopeFactoryMock.Object);

            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

            _service = new PortfolioKafkaConsumerService(_serviceProviderMock.Object, _kafkaConfigMock.Object, _loggerMock.Object, _emailConfigMock.Object);
        }

        //[Fact(DisplayName = "ExecuteAsync should process messages from Kafka")]
        //public async Task ExecuteAsync_ShouldProcessMessagesFromKafka()
        //{
        //    // Arrange
        //    var stoppingToken = new CancellationTokenSource().Token;

        //    _consumerMock.Setup(x => x.Consume(It.IsAny<CancellationToken>()))
        //        .Returns(new ConsumeResult<string, string>
        //        {
        //            Topic = KafkaTopics.InsertCustomerPorftolio,
        //            Message = new Message<string, string> { Key = "key", Value = "value" }
        //        });

        //    _serviceProviderMock.Setup(x => x.GetService(typeof(IConsumer<string, string>))).Returns(_consumerMock.Object);

        //    // Act
        //    await _service.StartAsync(stoppingToken);

        //    // Assert
        //    _consumerMock.Verify(x => x.Consume(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        //}

        [Fact(DisplayName = "ProcessMessageAsync should insert portfolio on InsertCustomerPorftolio topic")]
        public async Task ProcessMessageAsync_ShouldInsertPortfolio_OnInsertCustomerPorftolioTopic()
        {
            // Arrange
            var topic = KafkaTopics.InsertCustomerPorftolio;
            var key = "key";
            var value = JsonConvert.SerializeObject(new PortfolioRequest
            {
                CustomerId = 1,
                ProductId = Guid.NewGuid(),
                ProductName = "Product",
                AmountNegotiated = 10,
                ValueNegotiated = 100m
            });

            // Act
            await _service.ProcessMessageAsync(topic, key, value, _repositoryMock.Object, _emailNotificationServiceMock.Object, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<PortfolioRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "ProcessMessageAsync should remove portfolio on DeleteCustomerPorftolio topic")]
        public async Task ProcessMessageAsync_ShouldRemovePortfolio_OnDeleteCustomerPorftolioTopic()
        {
            // Arrange
            var topic = KafkaTopics.DeleteCustomerPorftolio;
            var key = "key";
            var value = JsonConvert.SerializeObject(new PortfolioRequest
            {
                CustomerId = 1,
                ProductId = Guid.NewGuid(),
                ProductName = "Product",
                AmountNegotiated = 10,
                ValueNegotiated = 100m
            });

            // Act
            await _service.ProcessMessageAsync(topic, key, value, _repositoryMock.Object, _emailNotificationServiceMock.Object, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.RemoveAsync(It.IsAny<PortfolioRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
