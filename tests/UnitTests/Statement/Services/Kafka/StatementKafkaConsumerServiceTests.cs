using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Infrastructure.Email;
using Infrastructure.Kafka;
using Infrastructure.Repository.Entities;
using Investments.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Statement.Repository.Interface;
using Statement.Service.Kafka;

namespace UnitTests.Statement.Services.Kafka
{

    public class StatementKafkaConsumerServiceTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IPortfolioStatementRepository> _repositoryMock;
        private readonly Mock<ILogger<StatementKafkaConsumerService>> _loggerMock;
        private readonly Mock<IConsumer<string, string>> _consumerMock;
        private readonly StatementKafkaConsumerService _service;
        private readonly KafkaConfig _kafkaConfig;
        private readonly EmailConfig _emailConfig;

        public StatementKafkaConsumerServiceTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _repositoryMock = new Mock<IPortfolioStatementRepository>();
            _loggerMock = new Mock<ILogger<StatementKafkaConsumerService>>();
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

            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPortfolioStatementRepository))).Returns(_repositoryMock.Object);
            _serviceProviderMock.Setup(sp => sp.GetService(typeof(IServiceScopeFactory))).Returns(_serviceScopeFactoryMock.Object);

            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

            _service = new StatementKafkaConsumerService(
                _serviceProviderMock.Object,
                Options.Create(_kafkaConfig),
                _loggerMock.Object,
                Options.Create(_emailConfig)
            );

            _service.GetType().GetField("_consumer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_service, _consumerMock.Object);
        }

        [Fact(DisplayName = "ProcessMessageAsync should insert portfolio statement on InsertCustomerPorftolioStatement topic")]
        public async Task ProcessMessageAsync_ShouldInsertPortfolioStatement_OnInsertCustomerPorftolioStatementTopic()
        {
            // Arrange
            var topic = KafkaTopics.InsertCustomerPorftolioStatement;
            var key = "key";
            var value = JsonConvert.SerializeObject(new PortfolioStatementDomain
            {
                CustomerId = 1,
                ProductId = Guid.NewGuid(),
                ProductName = "Product",
                AmountNegotiated = 10,
                OperationType = "BUY",
                TransactionDate = DateTime.UtcNow
            });

            // Act
            await _service.ProcessMessageAsync(topic, key, value, _repositoryMock.Object, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<PortfolioStatementDomain>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
