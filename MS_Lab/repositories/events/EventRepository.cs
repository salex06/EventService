using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MS_Lab.data;
using MS_Lab.entities;
using System.Net.Sockets;

namespace MS_Lab.repositories.events
{

    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<Event> _events;
        public EventRepository(IMongoDatabase db) {
            _events = db.GetCollection<Event>("events");
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            var filter = Builders<Event>.Filter.Empty;

            return await _events.Find(filter).ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(string id)
        {
            try
            {
                var filter = Builders<Event>.Filter.Eq(e => e.Id, id);

                return await _events.Find(filter).FirstOrDefaultAsync();
            }
            catch (FormatException) {
                return null;
            }
        }

        public async Task<Event> CreateAsync(Event eventEntity)
        {
            if (string.IsNullOrEmpty(eventEntity.Id))
            {
                eventEntity.Id = ObjectId.GenerateNewId().ToString();
            }

            await _events.InsertOneAsync(eventEntity);

            return eventEntity;
        }

        public async Task<Event?> UpdateAsync(Event eventEntity)
        {
            try
            {
                var filter = Builders<Event>.Filter.Eq(t => t.Id, eventEntity.Id);

                await _events.ReplaceOneAsync(filter, eventEntity);

                return eventEntity;
            }
            catch (FormatException) {
                return null;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var filter = Builders<Event>.Filter.Eq(t => t.Id, id);
                await _events.DeleteOneAsync(filter);
            }
            catch (FormatException) { 
                //It'll be better to log it
            }
        }

        public async Task<bool> ExistsByIdAsync(string id) {
            try
            {
                var filter = Builders<Event>.Filter.Eq(t => t.Id, id);

                var count = await _events.CountDocumentsAsync(filter, new CountOptions
                {
                    Limit = 1
                });

                return count > 0;
            }
            catch (FormatException) {
                return false;
            }
        }
    }
}