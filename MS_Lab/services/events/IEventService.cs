using MS_Lab.dto.events;

namespace MS_Lab.services.events
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetAllEventsAsync();
        Task<EventDTO> GetEventByIdAsync(int id);
        Task<EventDTO> CreateEventAsync(CreateEventDTO createEventDTO);
        Task<EventDTO> UpdateEventAsync(int eventId, UpdateEventDTO updateEventDTO);
        Task DeleteEventAsync(int id);
    }   

}
