using MS_Lab.entities;

namespace MS_Lab.repositories.events
{

    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(string id);
        Task<Event> CreateAsync(Event eventEntity);
        Task<Event?> UpdateAsync(Event eventEntity);
        Task DeleteAsync(string id);
        Task<bool> ExistsByIdAsync(string id);
    }

}