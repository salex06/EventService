using MS_Lab.dto;
using MS_Lab.dto.events;

namespace MS_Lab.services.events
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync(EventFilterDto filter);
        Task<EventDto> GetEventByIdAsync(string id);
        Task<EventDto> CreateEventAsync(CreateEventDto createEventDTO);
        Task<EventDto> UpdateEventAsync(string eventId, UpdateEventDto updateEventDTO);
        Task DeleteEventAsync(string id);
        Task UpdateConfirmationAsync(ConfirmedObjectDto confirmedObjectDto);
    }

}
