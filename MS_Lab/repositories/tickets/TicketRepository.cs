using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MS_Lab.config;
using MS_Lab.data;
using MS_Lab.entities;
using MS_Lab.specification;

namespace MS_Lab.repositories.tickets
{

    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly RepositoryConfig _repositoryConfig;
        public TicketRepository(IMongoDatabase db, IOptions<RepositoryConfig> settings)
        {
            _tickets = db.GetCollection<Ticket>("tickets");
            _repositoryConfig = settings.Value;
        }
        public async Task<IEnumerable<Ticket>> GetAllAsync(ISpecification<Ticket>? spec = null)
        {
            var filter = Builders<Ticket>.Filter.Empty;

            if (spec?.Criteria != null) {
                filter = Builders<Ticket>.Filter.Where(spec.Criteria);
            }

            return await _tickets.Aggregate()
                .Match(filter)
                .Sample(_repositoryConfig.ObjectPerRequestLimit)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(string id)
        {
            try
            {
                var filter = Builders<Ticket>.Filter.Eq(e => e.Id, id);

                return await _tickets.Find(filter).FirstOrDefaultAsync();
            }
            catch (FormatException)
            {
                return null;
            }
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

        public async Task<Ticket?> UpdateAsync(Ticket ticket)
        {
            try
            {
                var filter = Builders<Ticket>.Filter.Eq(t => t.Id, ticket.Id);

                await _tickets.ReplaceOneAsync(filter, ticket);

                return ticket;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var filter = Builders<Ticket>.Filter.Eq(t => t.Id, id);
                await _tickets.DeleteOneAsync(filter);
            }
            catch (FormatException)
            {
                //It'll be better to log it
            }
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            try
            {
                var filter = Builders<Ticket>.Filter.Eq(t => t.Id, id);

                var count = await _tickets.CountDocumentsAsync(filter, new CountOptions
                {
                    Limit = 1
                });

                return count > 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public async Task<long> GetSoldTicketNumberByEventIdAsync(string eventId)
        {
            try
            {
                return await _tickets.CountDocumentsAsync(t => t.Event.Id == eventId);
            }
            catch (FormatException)
            {
                return 0;
            }
        }
    }

}