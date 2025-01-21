using Confluent.Kafka;
using Infrastructure.Repository.Entities;
using Investments.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;
using IEmailNotificationService = Infrastructure.Email.Interface.IEmailNotificationService;
using Infrastructure.Email;
using Portfolio.Repository.Interface;


namespace Portfolio.Service.Kafka
{
    public class PortfolioKafkaConsumerService : BackgroundService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly EmailConfig _emailConfig;
        private readonly int _daysToExpiration = 7;
        private readonly IServiceProvider _serviceProvider; // Injeção de IServiceProvider
        private readonly ILogger<PortfolioKafkaConsumerService> _logger;
        private readonly List<string> _topics;
        private readonly IConsumer<string, string> _consumer;


        public PortfolioKafkaConsumerService(IServiceProvider serviceProvider, IOptions<KafkaConfig> kafkaConfig, ILogger<PortfolioKafkaConsumerService> logger, IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
            _kafkaConfig = kafkaConfig.Value;
            _serviceProvider = serviceProvider; // Guardar o IServiceProvider
            _logger = logger;
            // Lista de tópicos que o consumidor vai ler
            _topics = new List<string>
            {
                KafkaTopics.InsertCustomerPorftolio,
                KafkaTopics.DeleteCustomerPorftolio
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();
                var emailNotificationService = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();


                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(stoppingToken);
                            if (consumeResult != null)
                            {
                                _logger.LogInformation($"Mensagem recebida do tópico {consumeResult.Topic}: {consumeResult.Message.Value}");

                                await ProcessMessageAsync(consumeResult.Topic, consumeResult.Message.Key, consumeResult.Message.Value, repository, emailNotificationService, stoppingToken);

                                _consumer.Commit();
                            }
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError($"Erro ao consumir mensagem: {e.Message}");
                        }

                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Consumo de Kafka cancelado.");
                }
                finally
                {
                    _consumer.Close();
                }
            }
        }



        public async Task ProcessMessageAsync(string topic, string key, string value, IPortfolioRepository repository, IEmailNotificationService emailNotificationService, CancellationToken stoppingToken)
        {
            // Processamento da mensagem usando o repository scoped
            switch (topic)
            {
                case KafkaTopics.InsertCustomerPorftolio:
                    _logger.LogInformation($"Processando mensagem de atualização de portfolio. Key: {key}, Value: {value}");
                    var portfolioInsert = JsonConvert.DeserializeObject<PortfolioRequest>(value);
                    await repository.InsertAsync(portfolioInsert, stoppingToken);
                    break;

                case KafkaTopics.DeleteCustomerPorftolio:
                    _logger.LogInformation($"Processando mensagem de exclusão de portfolio. Key: {key}, Value: {value}");
                    var portfolioDelete = JsonConvert.DeserializeObject<PortfolioRequest>(value);
                    await repository.RemoveAsync(portfolioDelete, stoppingToken);
                    break;

                default:
                    _logger.LogWarning($"Tópico desconhecido: {topic}");
                    break;
            }
        }

    }

}