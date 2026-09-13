using MS_Lab.dto;
using MS_Lab.dto.events;
using MS_Lab.entities;

namespace MS_Lab.services.events
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync(EventFilterDto? filter);
        Task<Event> GetEventByIdAsync(string id);
        Task<Event> CreateEventAsync(Event eventInfo);
        Task<Event> UpdateEventAsync(string eventId, UpdateEventDto updateEvent);
        Task DeleteEventAsync(string id);
        Task UpdateConfirmationAsync(ConfirmedObjectDto confirmedObjectDto);
    }

}
