namespace MS_Lab.kafka.producer
{
    public interface IKafkaMessageProducer
    {
        Task<bool> SendMessageAsync(string topic, string message, string? key = null);
    }
}
