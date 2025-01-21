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
    public class DeleteProductEventHandler : INotificationHandler<DeleteProductEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IProductStatementRepository _repositoryWrite;
        public DeleteProductEventHandler(IKafkaProducerService kafkaProducerService, IProductStatementRepository repositoryWrite)
        {
            _repositoryWrite = repositoryWrite;
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(DeleteProductEvent productEvent, CancellationToken cancellationToken)
        {
            await _repositoryWrite.DeleteAsync(productEvent.Id, cancellationToken);
            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.DeleteProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent));
        }
    }
}
