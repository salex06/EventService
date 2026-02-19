using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MS_Lab.entities;

namespace MS_Lab.entities
{
    public class TicketOwner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("surname")]
        public string Surname { get; set; } = string.Empty;

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;
    }

}