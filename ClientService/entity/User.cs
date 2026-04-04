using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientService.entity
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("registered_objects")]
        public int RegisteredObjects { get; set; } = 0;
    }
}
