using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MS_Lab.dto.ticket;
using MS_Lab.entities;
using MS_Lab.exception;
using MS_Lab.repositories.events;
using MS_Lab.repositories.tickets;
using MS_Lab.specification;
using Prometheus;
using System.Text.Json;

namespace MS_Lab.services.tickets
{
    public class TicketService : ITicketService
    {
        private static readonly Counter createdTicketsCounter = Metrics
    .CreateCounter("created_tickets_total", "Created tickets count");

        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;

        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        // `время жизни` кэша в минтуах
        private readonly int _cacheExpirationMinutes = 5;

        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository, IMapper mapper, IDistributedCache cache)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync(TicketFilterDto filter)
        {
            var spec = TicketSpecification.FromFilter(filter);
            var tickets = await _ticketRepository.GetAllAsync(spec);
            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }

        public async Task<TicketDto> GetTicketByIdAsync(string id)
        {
            string cacheKey = $"ticket:{id}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<TicketDto>(cached)!;
            }

            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                throw new NotFoundException($"Билет с id={id} не найден");
            }

            var dto = _mapper.Map<TicketDto>(ticket);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), options);
            return dto;
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto createTicketDTO)
        {
            string eventId = createTicketDTO.EventId;
            var foundEvent = await _eventRepository.GetByIdAsync(eventId);
            if (foundEvent == null)
                throw new NotFoundException($"Событие с id={eventId} не найдено");

            var soldTicketNumber = await _ticketRepository.GetSoldTicketNumberByEventIdAsync(eventId);
            if (soldTicketNumber == foundEvent.TicketCount)
                throw new BadRequestException("Все билеты проданы");

            var ticket = _mapper.Map<Ticket>(createTicketDTO);
            ticket.Event = foundEvent;

            var savedTicket = await _ticketRepository.CreateAsync(ticket);
            var dto = _mapper.Map<TicketDto>(savedTicket);

            string cacheKey = $"ticket:{dto.Id}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), options);

            createdTicketsCounter.Inc();
            return dto;
        }

        public async Task<TicketDto> UpdateTicketAsync(string id, UpdateTicketDto updateTicketDTO)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(id);
            if (existingTicket == null)
                throw new NotFoundException($"Билет с id={id} не найден");

            var foundEvent = await _eventRepository.GetByIdAsync(existingTicket.Event.Id);
            if (foundEvent == null)
                throw new NotFoundException($"Событие с id={existingTicket.Event.Id} не найдено");

            _mapper.Map(updateTicketDTO, existingTicket);
            var updated = await _ticketRepository.UpdateAsync(existingTicket);
            var dto = _mapper.Map<TicketDto>(updated);

            string cacheKey = $"ticket:{dto.Id}";
            await _cache.RemoveAsync(cacheKey);

            return dto;
        }

        public async Task DeleteTicketAsync(string id)
        {
            if (!await _ticketRepository.ExistsByIdAsync(id))
            {
                throw new NotFoundException($"Билет с id={id} не найден");
            }

            await _ticketRepository.DeleteAsync(id);
            await _cache.RemoveAsync($"ticket:{id}");
        }
    }
}
