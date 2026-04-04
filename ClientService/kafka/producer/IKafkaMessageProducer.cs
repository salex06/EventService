namespace ClientService.kafka.producer
{
    public interface IKafkaMessageProducer
    {
        Task<bool> SendMessageAsync(string topic, string message, string? key = null);
    }
}
