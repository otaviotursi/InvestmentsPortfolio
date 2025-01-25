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


namespace Products.Service.Kafka
{
    public class ProductKafkaConsumerService : BackgroundService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly EmailConfig _emailConfig;
        private readonly int _daysToExpiration = 7;
        private readonly IServiceProvider _serviceProvider; // Injeção de IServiceProvider
        private readonly ILogger<ProductKafkaConsumerService> _logger;
        private readonly List<string> _topics;
        private readonly IConsumer<string, string> _consumer;


        public ProductKafkaConsumerService(IServiceProvider serviceProvider, IOptions<KafkaConfig> kafkaConfig, ILogger<ProductKafkaConsumerService> logger, IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
            _kafkaConfig = kafkaConfig.Value;
            _serviceProvider = serviceProvider; // Guardar o IServiceProvider
            _logger = logger;
            // Lista de tópicos que o consumidor vai ler
            _topics = new List<string>
            {
                KafkaTopics.InsertProductTopic,
                KafkaTopics.DeleteProductTopic,
                KafkaTopics.InvestmentPurchasedTopic,
                KafkaTopics.InvestmentSoldTopic,
                KafkaTopics.UpdateProductTopic,
                KafkaTopics.ProductExpiryNotificationTopic
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
                                var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                                var emailNotificationService = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();
                                await ProcessMessageAsync(consumeResult.Topic, consumeResult.Message.Key, consumeResult.Message.Value, repository, emailNotificationService, stoppingToken);
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



        private async Task ProcessMessageAsync(string topic, string key, string value, IProductRepository repository, IEmailNotificationService emailNotificationService, CancellationToken stoppingToken)
        {
            // Processamento da mensagem usando o repository scoped
            switch (topic)
            {
                case KafkaTopics.InsertProductTopic:
                    _logger.LogInformation($"Processando mensagem de inserção de Produto. Key: {key}, Value: {value}");
                    var productInsert = JsonConvert.DeserializeObject<ProductDomain>(value);
                    await repository.InsertAsync(productInsert, stoppingToken);
                    break;

                case KafkaTopics.UpdateProductTopic:
                    _logger.LogInformation($"Processando mensagem de atualização de Produto. Key: {key}, Value: {value}");
                    var productUpdate = JsonConvert.DeserializeObject<ProductDomain>(value);
                    await repository.UpdateAsync(productUpdate, stoppingToken);
                    break;

                case KafkaTopics.DeleteProductTopic:
                    _logger.LogInformation($"Processando mensagem de exclusão de Produto. Key: {key}, Value: {value}");
                    var productDelete = JsonConvert.DeserializeObject<ProductDomain>(value);
                    await repository.DeleteAsync(productDelete.Id, stoppingToken);
                    break;

                case KafkaTopics.ProductExpiryNotificationTopic:
                    _logger.LogInformation($"Processando mensagem de validação de expiração de produtos");
                    await SendProductExpirationEmail(repository, emailNotificationService, stoppingToken);
                    break;

                default:
                    _logger.LogWarning($"Tópico desconhecido: {topic}");
                    break;
            }
        }

        private async Task SendProductExpirationEmail(IProductRepository repository, IEmailNotificationService emailNotificationService, CancellationToken stoppingToken)
        {
            var listProducts = await repository.GetExpiritionByDateAll(_daysToExpiration, stoppingToken);
            StringBuilder emailBody = new StringBuilder();
            foreach (var product in listProducts)
            {
                TimeSpan diferenca = product.ExpirationDate - DateTime.Now;

                if (diferenca.TotalDays <= 7 && diferenca.TotalDays >= 0)
                    emailBody.Append($"Produto id {product.Id} - {product.Name}, Está para expirar em: {product.ExpirationDate}");
            }
            if (emailBody.Length > 0)
            {
                var emailRequest = new EmailRequest(_emailConfig.EmailSendExpiration, "Produtos prestes a expirar", emailBody.ToString());
                await emailNotificationService.SendEmailAsync(emailRequest);
            }
        }
    }

}