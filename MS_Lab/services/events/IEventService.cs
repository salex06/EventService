using MS_Lab.dto.events;

namespace MS_Lab.services.events
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetAllEvents();
        Task<EventDTO> GetEventById(int id);
        Task<EventDTO> CreateEvent(CreateEventDTO createEventDTO);
        Task<EventDTO> UpdateEvent(UpdateEventDTO updateEventDTO);
        Task DeleteEvent(int id);
    }   

}
