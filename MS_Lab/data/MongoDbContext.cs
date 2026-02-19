using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MS_Lab.entities;

namespace MS_Lab.data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Ticket> Tickets =>
            _database.GetCollection<Ticket>("tickets");

        public IMongoCollection<Event> Events =>
            _database.GetCollection<Event>("events");
    }

    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
