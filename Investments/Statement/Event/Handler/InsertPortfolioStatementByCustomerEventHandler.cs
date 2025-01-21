using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using MediatR;
using Newtonsoft.Json;
using Statement.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statement.Command.Handler
{
    public class InsertPortfolioStatementByCustomerEventHandler : INotificationHandler<InsertPortfolioStatementByCustomerEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        public InsertPortfolioStatementByCustomerEventHandler(IKafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(InsertPortfolioStatementByCustomerEvent productEvent, CancellationToken cancellationToken)
        {
            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.InsertCustomerPorftolioStatement, productEvent.CustomerId.ToString(), JsonConvert.SerializeObject(productEvent));
        }
    }
}
