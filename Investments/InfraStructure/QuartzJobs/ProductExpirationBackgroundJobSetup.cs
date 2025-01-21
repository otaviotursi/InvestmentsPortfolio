using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.QuartzJobs
{
    public class ProductExpirationBackgroundJob(
        ILogger<ProductExpirationBackgroundJob> logger,
         Services.IKafkaProducerService kafkaProducerService) : IJob
            {

        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Job iniciado");

            await kafkaProducerService.PublishMessageAsync(Investments.Infrastructure.Kafka.KafkaTopics.ProductExpiryNotificationTopic);

        }
    }
    public class ProductExpirationBackgroundJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            //var emailJobKey = JobKey.Create(nameof(ProductExpirationBackgroundJob));
            //options
            //    .AddJob<ProductExpirationBackgroundJob>(jobBuilder => jobBuilder.WithIdentity(emailJobKey))
            //    .AddTrigger(trigger =>
            //        trigger
            //            .ForJob(emailJobKey)
            //            .WithCronSchedule("0 0 6 ? * * *"))
            //    .AddTrigger(trigger =>
            //        trigger
            //            .ForJob(emailJobKey)
            //            .WithSimpleSchedule(schedule => schedule.WithIntervalInHours(8).RepeatForever()));
        }
    }
}
