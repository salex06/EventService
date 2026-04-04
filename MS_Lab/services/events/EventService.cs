using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using MS_Lab.dto;
using MS_Lab.dto.events;
using MS_Lab.entities;
using MS_Lab.exception;
using MS_Lab.repositories.events;
using MS_Lab.specification;
using Prometheus;
using System.Text.Json;

namespace MS_Lab.services.events
{
    public class EventService : IEventService
    {
        private static readonly Counter createdEventsCounter = Metrics
            .CreateCounter("created_events_total", "Created events count");

        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        // `время жизни` кэша в минтуах
        private readonly int _cacheExpirationMinutes = 5;

        public EventService(IEventRepository eventRepository, IMapper mapper, IDistributedCache cache)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync(EventFilterDto filter)
        {
            var spec = EventSpecification.FromFilter(filter);

            var events = await _eventRepository.GetAllAsync(spec);

            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetEventByIdAsync(string id)
        {
            string cacheKey = $"event:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<EventDto>(cached)!;
            }

            var foundEvent = await _eventRepository.GetByIdAsync(id);
            if (foundEvent == null)
            {
                throw new NotFoundException($"Событие с id={id} не найдено");
            }

            var dto = _mapper.Map<EventDto>(foundEvent);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), options);

            return dto;
        }


        public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDTO)
        {
            var eventInfo = _mapper.Map<Event>(createEventDTO);
            var savedEvent = await _eventRepository.CreateAsync(eventInfo);
            var dto = _mapper.Map<EventDto>(savedEvent);

            string cacheKey = $"event:{dto.Id}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), options);

            createdEventsCounter.Inc();
            return dto;
        }

        public async Task<EventDto> UpdateEventAsync(string eventId, UpdateEventDto updateEventDTO)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null)
                throw new NotFoundException($"Событие с id={eventId} не найдено");

            updateEventDTO.Id = eventId;
            _mapper.Map(updateEventDTO, existingEvent);
            
            var updatedEvent = await _eventRepository.UpdateAsync(existingEvent);
            var dto = _mapper.Map<EventDto>(updatedEvent);

            string cacheKey = $"event:{dto.Id}";
            await _cache.RemoveAsync(cacheKey);

            return dto;
        }

        public async Task DeleteEventAsync(string id)
        {
            if (!await _eventRepository.ExistsByIdAsync(id))
            {
                throw new NotFoundException($"Событие с id={id} не найдено");
            }

            await _eventRepository.DeleteAsync(id);
            await _cache.RemoveAsync($"event:{id}");
        }

        public async Task UpdateConfirmationAsync(ConfirmedObjectDto confirmedObjectDto) {
            var objId = confirmedObjectDto.ObjId;
            var foundEvent = await _eventRepository.GetByIdAsync(objId);
            if (foundEvent != null) {
                foundEvent.ConfirmStatus = ConfirmStatus.CONFFIRMED;
                foundEvent.ConfirmedAt = confirmedObjectDto.ConfirmDateTime;
                foundEvent.ConfirmatorId = confirmedObjectDto.ConfirmatorId;

                await _eventRepository.UpdateAsync(foundEvent);
            }
        }
    }
}
