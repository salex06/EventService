using MS_Lab.dto.events;
using MS_Lab.entities;
using MS_Lab.repositories;

namespace MS_Lab.services.events
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventDTO>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<EventDTO>>(events);
        }

        public async Task<EventDTO> GetEventByIdAsync(int id)
        {
            var foundEvent = await _eventRepository.GetByIdAsync(id);
            if (foundEvent == null) {
                throw NotFoundException($"Событие с id={id} не найдено");
            }

            return _mapper.Map<EventDTO>(foundEvent);
        }

        public async Task<EventDTO> CreateEventAsync(CreateEventDTO createEventDTO)
        {
            var eventInfo = _mapper.Map<Event>(createEventDTO);

            var savedEvent = await _eventRepository.CreateAsync(eventInfo);

            return _mapper.Map<EventDTO>(savedEvent);
        }

        public async Task<EventDTO> UpdateEventAsync(int eventId, UpdateEventDTO updateEventDTO)
        {
            var foundEvent = await _eventRepository.GetByIdAsync(eventId);
            if (foundEvent == null) {
                throw NotFoundException($"Событие с id={id} не найдено");
            }

            var eventToUpdate = _mapper.Map<Event>(updateEventDTO);
            eventToUpdate.Id = eventId;

            var updatedEvent = await _eventRepository.UpdateAsync(eventToUpdate);

            return _mapper.Map<EventDTO>(updatedEvent);
        }

        public async Task DeleteEventAsync(int id)
        {
            if (!await _eventRepository.ExistsByIdAsync(id))
            {
                throw NotFoundException($"Событие с id={id} не найдено");
            }

            await _eventRepository.DeleteAsync(id);
        }
    }
}
