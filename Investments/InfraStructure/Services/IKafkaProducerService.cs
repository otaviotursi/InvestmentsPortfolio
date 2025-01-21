namespace Infrastructure.Services
{
    public interface IKafkaProducerService
    {
        Task<bool> PublishMessageAsync(string topic, string key, string message);
        Task<bool> PublishMessageAsync(string topic);

    }
}
