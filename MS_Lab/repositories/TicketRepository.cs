using Microsoft.EntityFrameworkCore;
using MS_Lab.data;
using MS_Lab.entities;

namespace MS_Lab.repositories
{

    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;
        public TicketRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Ticket>> GetAllAsync() =>
            await _context.Tickets
                .Include(t => t.Owner)
                .Include(t => t.Event)
                .ToListAsync();

        public async Task<Ticket?> GetByIdAsync(int id) =>
            await _context.Tickets
                .Include(t => t.Owner)
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task DeleteAsync(int id)
        {
            var ticket = await GetByIdAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByIdAsync(int id) =>
            await _context.Tickets.AnyAsync(t => t.Id == id);

        public async Task<int> GetSoldTicketNumberByEventIdAsync(int eventId)
        {
            return await _context.Tickets.CountAsync(t => t.EventId == eventId);
        }
    }

}