using MS_Lab.dto;
using MS_Lab.dto.events;
using MS_Lab.dto.ticket;
using System.Text.Json;

namespace MS_Lab.kafka.producer
{
    public interface IKafkaMessageProducer
    {
        Task<bool> SendMessageAsync(string topic, string message, string? key = null);

        public void SendConfirmationRequest(dto.ObjectType type, string id, string confirmatorId, string topicName)
        {
            RegObjectDto regObject = new RegObjectDto()
            {
                Type = dto.ObjectType.Event,
                ObjectId = id,
                ConfirmatorId = confirmatorId
            };

            string message = JsonSerializer.Serialize(regObject);

            SendMessageAsync(topicName, message);
        }
    }
}
