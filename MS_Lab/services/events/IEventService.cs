using MS_Lab.dto.events;

namespace MS_Lab.services.events
{
    public interface IEventService
    {
        Task<IEnumerable<EventDTO>> GetAllEventsAsync();
        Task<EventDTO> GetEventByIdAsync(string id);
        Task<EventDTO> CreateEventAsync(CreateEventDTO createEventDTO);
        Task<EventDTO> UpdateEventAsync(string eventId, UpdateEventDTO updateEventDTO);
        Task DeleteEventAsync(string id);
    }   

}
