using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MS_Lab.entities
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("event")]
        public Event Event { get; set; } = new Event();

        [BsonElement("owner")]
        public TicketOwner Owner { get; set; } = new TicketOwner();

        [BsonElement("ticket_number")]
        public string TicketNumber { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("purchase_date")]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    }
}