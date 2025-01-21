
using Confluent.Kafka;
using Infrastructure.Kafka;
using Investments.Infrastructure.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;


namespace Infrastructure.Services
{
    public class KafkaPublisherService : IKafkaProducerService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly ILogger<KafkaPublisherService> _logger;

        public KafkaPublisherService(IOptions<KafkaConfig> kafkaConfig, ILogger<KafkaPublisherService> logger)
        {
            _kafkaConfig = kafkaConfig.Value ;
            _logger = logger;
        }

        public async Task<bool> PublishMessageAsync(string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                Acks = KafkaConfig.ParseAcks(_kafkaConfig.ProducerConfig.Acks),  // Converter Acks
                EnableIdempotence = _kafkaConfig.ProducerConfig.EnableIdempotence,
                LingerMs = _kafkaConfig.ProducerConfig.LingerMs,
                BatchNumMessages = _kafkaConfig.ProducerConfig.BatchNumMessages
            };

            using var producer = new ProducerBuilder<string, string>(config).Build();
            try
            {
                var result = await producer.ProduceAsync(topic, new Message<string, string> { });

                _logger.LogInformation($"Mensagem enviada para o tópico {topic}. Offset: {result.Offset}");
                return true;
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError($"Falha ao enviar mensagem para o tópico {topic}: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> PublishMessageAsync(string topic, string key, string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                Acks = KafkaConfig.ParseAcks(_kafkaConfig.ProducerConfig.Acks),  // Converter Acks
                EnableIdempotence = _kafkaConfig.ProducerConfig.EnableIdempotence,
                LingerMs = _kafkaConfig.ProducerConfig.LingerMs,
                BatchNumMessages = _kafkaConfig.ProducerConfig.BatchNumMessages
            };

            using var producer = new ProducerBuilder<string, string>(config).Build();
            try
            {
                var result = await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = message });

                _logger.LogInformation($"Mensagem enviada para o tópico {topic}. Offset: {result.Offset}");
                return true;
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError($"Falha ao enviar mensagem para o tópico {topic}: {ex.Message}");
                return false;
            }
        }
    }
}
