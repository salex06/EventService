using MS_Lab.entities;

namespace MS_Lab.repositories
{

    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(int id);
        Task<Ticket> CreateAsync(Ticket ticket);
        Task<Ticket> UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task<int> GetSoldTicketNumberByEventIdAsync(int eventId);
    }
}