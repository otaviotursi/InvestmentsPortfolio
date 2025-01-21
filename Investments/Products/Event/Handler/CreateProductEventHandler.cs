using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using MediatR;
using Newtonsoft.Json;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Event.Handler
{
    public class CreateProductEventHandler : INotificationHandler<CreateProductEvent>
    {
        private readonly IProductStatementRepository _repositoryWrite;
        private readonly IKafkaProducerService _kafkaProducerService;
        public CreateProductEventHandler(IKafkaProducerService kafkaProducerService, IProductStatementRepository repositoryWrite)
        {
            _repositoryWrite = repositoryWrite;
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(CreateProductEvent productEvent, CancellationToken cancellationToken)
        {

            productEvent.Type = "Create";
            await _repositoryWrite.InsertAsync(productEvent, cancellationToken);
            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.InsertProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent));
        }
    }
}
