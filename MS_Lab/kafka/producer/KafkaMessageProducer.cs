using Confluent.Kafka;
using Prometheus;

namespace MS_Lab.kafka.producer
{
    public class KafkaMessageProducer : IKafkaMessageProducer
    {
        private static readonly Counter requestedObjectsCounter = Metrics
.CreateCounter("obj_serv_obj_requested", "Requested for confirmation object count");

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

                requestedObjectsCounter.Inc();

                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
}
