using MS_Lab.entities;
using MS_Lab.specification;

namespace MS_Lab.repositories.events
{

    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync(ISpecification<Event>? spec = null);
        Task<Event?> GetByIdAsync(string id);
        Task<Event> CreateAsync(Event eventEntity);
        Task<Event?> UpdateAsync(Event eventEntity);
        Task DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
    }

}