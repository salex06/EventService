using MS_Lab.entities;
using MS_Lab.specification;

namespace MS_Lab.repositories.tickets
{

    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync(ISpecification<Ticket>? spec = null);
        Task<Ticket?> GetByIdAsync(string id);
        Task<Ticket> CreateAsync(Ticket ticket);
        Task<Ticket?> UpdateAsync(Ticket ticket);
        Task DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
        Task<long> GetSoldTicketNumberByEventIdAsync(string eventId);
    }
}