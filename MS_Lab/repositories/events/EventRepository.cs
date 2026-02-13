using Microsoft.EntityFrameworkCore;
using MS_Lab.data;
using MS_Lab.entities;

namespace MS_Lab.repositories.events
{

    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;
        public EventRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Event>> GetAllAsync() =>
            await _context.Events.ToListAsync();

        public async Task<Event?> GetByIdAsync(int id) =>
            await _context.Events.FindAsync(id);

        public async Task<Event> CreateAsync(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<Event> UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Events.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByIdAsync(int id) =>
            await _context.Events.AnyAsync(e => e.Id == id);
    }
}