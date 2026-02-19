using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MS_Lab.data;
using MS_Lab.entities;

namespace MS_Lab.repositories.tickets
{

    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;
        public TicketRepository(IMongoDatabase db)
        {
            _tickets = db.GetCollection<Ticket>("tickets");
        }
        public async Task<IEnumerable<Ticket>> GetAllAsync() {
            var filter = Builders<Ticket>.Filter.Empty;

            return await _tickets.Find(filter).ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(string id) {
            var filter = Builders<Ticket>.Filter.Eq(e => e.Id, id);

            return await _tickets.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            if (string.IsNullOrEmpty(ticket.Id))
            {
                ticket.Id = ObjectId.GenerateNewId().ToString();
            }

            await _tickets.InsertOneAsync(ticket);

            return ticket;
        }

        public async Task<Ticket> UpdateAsync(Ticket ticket)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticket.Id);

            await _tickets.ReplaceOneAsync(filter, ticket);

            return ticket;
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, id);
            await _tickets.DeleteOneAsync(filter);
        }

        public async Task<bool> ExistsByIdAsync(string id) {
            var filter = Builders<Ticket>.Filter.Eq(t => t.Id, id);

            var count = await _tickets.CountDocumentsAsync(filter, new CountOptions
            {
                Limit = 1
            });

            return count > 0;
        }

        public async Task<long> GetSoldTicketNumberByEventIdAsync(string eventId)
        {
            return await _tickets.CountDocumentsAsync(t => t.Event.Id == eventId);
        }
    }

}