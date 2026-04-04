using Confluent.Kafka;

namespace ClientService.kafka.producer
{
    public class KafkaMessageProducer : IKafkaMessageProducer
    {
        private readonly IProducer<string, string> _producer;
        public KafkaMessageProducer(IProducer<string, string> producer)
        {
            _producer = producer;
        }
        public async Task<bool> SendMessageAsync(string topic, string message, string? key = null)
        {
            try
            {
                var kafkaMessage = new Message<string, string>
                {
                    Key = key ?? Guid.NewGuid().ToString(),
                    Value = message,
                    Timestamp = new Confluent.Kafka.Timestamp(DateTime.UtcNow)
                };

                await _producer.ProduceAsync(topic, kafkaMessage);

                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
}
