using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using MediatR;
using Newtonsoft.Json;
using Portfolio.Repository.Interface;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Event.Handler
{
    public class DeletePortfolioEventHandler : INotificationHandler<DeletePortfolioEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        public DeletePortfolioEventHandler(IKafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(DeletePortfolioEvent portfolio, CancellationToken cancellationToken)
        {

            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.InsertCustomerPorftolio, portfolio.CustomerId + " - " + portfolio.ProductId, JsonConvert.SerializeObject(portfolio));
        }
    }
}
