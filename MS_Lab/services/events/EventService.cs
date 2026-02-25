using AutoMapper;
using MS_Lab.dto.events;
using MS_Lab.exception;
using MS_Lab.entities;
using MS_Lab.repositories.events;

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

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetEventByIdAsync(string id)
        {
            var foundEvent = await _eventRepository.GetByIdAsync(id);
            if (foundEvent == null)
            {
                throw new NotFoundException($"Событие с id={id} не найдено");
            }

            return _mapper.Map<EventDto>(foundEvent);
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDTO)
        {
            var eventInfo = _mapper.Map<Event>(createEventDTO);

            var savedEvent = await _eventRepository.CreateAsync(eventInfo);

            return _mapper.Map<EventDto>(savedEvent);
        }

        public async Task<EventDto> UpdateEventAsync(string eventId, UpdateEventDto updateEventDTO)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null)
                throw new NotFoundException($"Событие с id={eventId} не найдено");

            updateEventDTO.Id = eventId;
            _mapper.Map(updateEventDTO, existingEvent);

            var updatedEvent = await _eventRepository.UpdateAsync(existingEvent);
            return _mapper.Map<EventDto>(updatedEvent);
        }

        public async Task DeleteEventAsync(string id)
        {
            if (!await _eventRepository.ExistsByIdAsync(id))
            {
                throw new NotFoundException($"Событие с id={id} не найдено");
            }

            await _eventRepository.DeleteAsync(id);
        }
    }
}
