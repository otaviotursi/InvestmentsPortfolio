using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using MediatR;
using Newtonsoft.Json;

namespace Portfolio.Event.Handler
{
    public class InsertPortfolioEventHandler : INotificationHandler<InsertPortfolioEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        public InsertPortfolioEventHandler(IKafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(InsertPortfolioEvent portfolio, CancellationToken cancellationToken)
        {

            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.InsertCustomerPorftolio, portfolio.CustomerId + " - " + portfolio.ProductId, JsonConvert.SerializeObject(portfolio));
        }
    }
}
