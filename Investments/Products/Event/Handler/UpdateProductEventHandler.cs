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
    public class UpdateProductEventHandler : INotificationHandler<UpdateProductEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IProductStatementRepository _repositoryWrite;
        public UpdateProductEventHandler(IKafkaProducerService kafkaProducerService, IProductStatementRepository repositoryWrite)
        {
            _repositoryWrite = repositoryWrite;
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(UpdateProductEvent productEvent, CancellationToken cancellationToken)
        {
            productEvent.Type = "Update";
            await _repositoryWrite.UpdateAsync(productEvent, cancellationToken);
            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.UpdateProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent));
        }
    }
}
