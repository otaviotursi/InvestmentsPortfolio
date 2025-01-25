using Confluent.Kafka;
using Infrastructure.Repository.Entities;
using Investments.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;
using IEmailNotificationService = Infrastructure.Email.Interface.IEmailNotificationService;
using Infrastructure.Email;
using Statement.Repository.Interface;


namespace Statement.Service.Kafka
{
    public class StatementKafkaConsumerService : BackgroundService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly EmailConfig _emailConfig;
        private readonly int _daysToExpiration = 7;
        private readonly IServiceProvider _serviceProvider; // Injeção de IServiceProvider
        private readonly ILogger<StatementKafkaConsumerService> _logger;
        private readonly List<string> _topics;
        private readonly IConsumer<string, string> _consumer;


        public StatementKafkaConsumerService(IServiceProvider serviceProvider, IOptions<KafkaConfig> kafkaConfig, ILogger<StatementKafkaConsumerService> logger, IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
            _kafkaConfig = kafkaConfig.Value;
            _serviceProvider = serviceProvider; // Guardar o IServiceProvider
            _logger = logger;
            // Lista de tópicos que o consumidor vai ler
            _topics = new List<string>
            {
                KafkaTopics.InsertCustomerPorftolioStatement
            };
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                GroupId = _kafkaConfig.ConsumerGroupId, // Adicionar um GroupId para o consumidor
                AutoOffsetReset = AutoOffsetReset.Earliest, // Garantir que comece do início se não houver offsets salvos
                EnableAutoCommit = false // Commit manual após processamento
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _consumer.Subscribe(_topics);
            _logger.LogInformation($"Inscrito nos tópicos: {string.Join(", ", _topics)}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(10)); // Tempo limite configurado
                        if (consumeResult != null)
                        {
                            _logger.LogInformation($"Mensagem recebida do tópico {consumeResult.Topic}: {consumeResult.Message.Value}");

                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var repository = scope.ServiceProvider.GetRequiredService<IPortfolioStatementRepository>();
                                await ProcessMessageAsync(consumeResult.Topic, consumeResult.Message.Key, consumeResult.Message.Value, repository, stoppingToken);
                            }

                            _consumer.Commit();
                            _logger.LogInformation("Mensagem processada e commit realizado.");
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Erro ao consumir mensagem: {e.Message}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro inesperado no processamento: {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken); // Pausa entre consumos
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumo de Kafka cancelado.");
            }
            finally
            {
                _consumer.Close();
                _logger.LogInformation("Consumer Kafka encerrado.");
            }
        }



        public async Task ProcessMessageAsync(string topic, string key, string value, IPortfolioStatementRepository repository, CancellationToken stoppingToken)
        {
            // Processamento da mensagem usando o repository scoped
            switch (topic)
            {
                case KafkaTopics.InsertCustomerPorftolioStatement:
                    _logger.LogInformation($"Processando mensagem de inserção de portfolio no extrato. Key: {key}, Value: {value}");
                    var insertProduct = JsonConvert.DeserializeObject<PortfolioStatementDomain>(value);
                    await repository.InsertAsync(insertProduct, stoppingToken);
                    break;

                default:
                    _logger.LogWarning($"Tópico desconhecido: {topic}");
                    break;
            }
        }

    }

}