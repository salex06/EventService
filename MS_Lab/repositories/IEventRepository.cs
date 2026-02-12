using MS_Lab.entities;

namespace MS_Lab.repositories
{

    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateAsync(Event eventEntity);
        Task<Event> UpdateAsync(Event eventEntity);
        Task DeleteAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
    }

}