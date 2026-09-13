using MS_Lab.dto;
using MS_Lab.dto.events;
using System.Text.Json;

namespace MS_Lab.kafka.producer
{
    public interface IKafkaMessageProducer
    {
        Task<bool> SendMessageAsync(string topic, string message, string? key = null);

        public void SendConfirmationRequest(EventDto createdEvent, string confirmatorId, string topicName)
        {
            RegObjectDto regObject = new RegObjectDto()
            {
                Type = dto.ObjectType.Event,
                ObjectId = createdEvent.Id,
                ConfirmatorId = confirmatorId
            };

            string message = JsonSerializer.Serialize(regObject);

            SendMessageAsync(topicName, message);
        }
    }
}
