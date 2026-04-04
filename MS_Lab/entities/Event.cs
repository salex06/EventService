using MS_Lab.enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MS_Lab.entities
{

    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("place")]
        public string Place { get; set; } = string.Empty;

        [BsonElement("event_type")]
        public EventType EventType { get; set; }  // enum как int

        [BsonElement("start_time_utc")]
        public DateTime StartTimeUTC { get; set; }

        [BsonElement("end_time_utc")]
        public DateTime EndTimeUTC { get; set; }

        [BsonElement("ticket_count")]
        public int TicketCount { get; set; }

        [BsonElement("price")]
        public int Price { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("confirmator_id")]
        public string ConfirmatorId { get; set; } = "";

        [BsonElement("confirm_status")]
        public ConfirmStatus ConfirmStatus { get; set; } = ConfirmStatus.NOT_CONFIRMED;

        [BsonElement("confirmed_at")]
        public DateTime? ConfirmedAt { get; set; }
    }

}