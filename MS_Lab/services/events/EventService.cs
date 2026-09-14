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
        private readonly IDistributedCache _cache;

        // `время жизни` кэша в минтуах
        private readonly int _cacheExpirationMinutes = 5;

        public EventService(IEventRepository eventRepository, IDistributedCache cache)
        {
            _eventRepository = eventRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync(EventFilterDto? filter)
        {
            var spec = EventSpecification.FromFilter(filter);

            return await _eventRepository.GetAllAsync(spec);
        }

        public async Task<Event> GetEventByIdAsync(string id)
        {
            string cacheKey = $"event:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<Event>(cached)!;
            }

            var foundEvent = await _eventRepository.GetByIdAsync(id);
            if (foundEvent == null)
            {
                throw new NotFoundException($"Событие с id={id} не найдено");
            }

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(foundEvent), options);

            return foundEvent;
        }


        public async Task<Event> CreateEventAsync(Event eventInfo)
        {
            var savedEvent = await _eventRepository.CreateAsync(eventInfo);

            string cacheKey = $"event:{savedEvent.Id}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(savedEvent), options);

            createdEventsCounter.Inc();

            return savedEvent;
        }

        public async Task<Event> UpdateEventAsync(string eventId, UpdateEventDto updateEvent)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null)
                throw new NotFoundException($"Событие с id={eventId} не найдено");

            string cacheKey = $"event:{updateEvent.Id}";
            await _cache.RemoveAsync(cacheKey);

            if (updateEvent.Name != null) existingEvent.Name = updateEvent.Name;
            if (updateEvent.Place != null) existingEvent.Place = updateEvent.Place;
            if (updateEvent.Description != null) existingEvent.Description = updateEvent.Description;
            if (updateEvent.EndTimeUTC != null) existingEvent.EndTimeUTC = (DateTime)updateEvent.EndTimeUTC;
            if (updateEvent.EventType != null) existingEvent.EventType = (enums.EventType)updateEvent.EventType;
            if (updateEvent.Price != null) existingEvent.Price = (int)updateEvent.Price;
            if (updateEvent.StartTimeUTC != null) existingEvent.StartTimeUTC = (DateTime)updateEvent.StartTimeUTC;
            if (updateEvent.TicketCount != null) existingEvent.TicketCount = (int)updateEvent.TicketCount;

            return await _eventRepository.UpdateAsync(existingEvent);
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

        public async Task UpdateConfirmationAsync(ConfirmedObjectDto confirmedObjectDto)
        {
            var objId = confirmedObjectDto.ObjId;
            var foundEvent = await _eventRepository.GetByIdAsync(objId);
            if (foundEvent != null)
            {
                foundEvent.ConfirmStatus = ConfirmStatus.CONFFIRMED;
                foundEvent.ConfirmedAt = confirmedObjectDto.ConfirmDateTime;
                foundEvent.ConfirmatorId = confirmedObjectDto.ConfirmatorId;

                await _eventRepository.UpdateAsync(foundEvent);
            }
        }
    }
}
