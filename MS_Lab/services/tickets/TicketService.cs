using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MS_Lab.dto;
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

        //private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        // `время жизни` кэша в минтуах
        private readonly int _cacheExpirationMinutes = 5;

        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository, IDistributedCache cache)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(TicketFilterDto filter)
        {
            var spec = TicketSpecification.FromFilter(filter);
            //return _mapper.Map<IEnumerable<TicketDto>>(tickets);
            return await _ticketRepository.GetAllAsync(spec);
        }

        public async Task<Ticket> GetTicketByIdAsync(string id)
        {
            string cacheKey = $"ticket:{id}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                return JsonSerializer.Deserialize<Ticket>(cached)!;
            }

            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                throw new NotFoundException($"Билет с id={id} не найден");
            }

            //var dto = _mapper.Map<TicketDto>(ticket);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(ticket), options);
            return ticket;
        }

        public async Task<Ticket> CreateTicketAsync(CreateTicketDto createTicketDTO)
        {
            string eventId = createTicketDTO.EventId;
            var foundEvent = await _eventRepository.GetByIdAsync(eventId);
            if (foundEvent == null)
                throw new NotFoundException($"Событие с id={eventId} не найдено");

            var soldTicketNumber = await _ticketRepository.GetSoldTicketNumberByEventIdAsync(eventId);
            if (soldTicketNumber == foundEvent.TicketCount)
                throw new BadRequestException("Все билеты проданы");

            TicketOwner owner = new TicketOwner {
                Id = createTicketDTO.TicketOwner.Id,
                Name = createTicketDTO.TicketOwner.Name,
                Surname = createTicketDTO.TicketOwner.Surname,
                Phone = createTicketDTO.TicketOwner.Phone,
                Email = createTicketDTO.TicketOwner.Email
            };

            Ticket ticket = new Ticket
            {
                Event = foundEvent,
                Owner = owner,
                ConfirmatorId = createTicketDTO.ConfirmatorId
            };

            //var ticket = _mapper.Map<Ticket>(createTicketDTO);
            //ticket.Event = foundEvent;

            var savedTicket = await _ticketRepository.CreateAsync(ticket);
            //var dto = _mapper.Map<TicketDto>(savedTicket);

            string cacheKey = $"ticket:{savedTicket.Id}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(savedTicket), options);

            createdTicketsCounter.Inc();
            return savedTicket;
        }

        public async Task<Ticket> UpdateTicketAsync(string id, UpdateTicketDto updateTicketDTO)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(id);
            if (existingTicket == null)
                throw new NotFoundException($"Билет с id={id} не найден");

            var foundEvent = await _eventRepository.GetByIdAsync(existingTicket.Event.Id);
            if (foundEvent == null)
                throw new NotFoundException($"Событие с id={existingTicket.Event.Id} не найдено");


            if (updateTicketDTO.TicketOwner != null)
            {
                if (updateTicketDTO.TicketOwner.Id != null) existingTicket.Owner.Id = updateTicketDTO.TicketOwner.Id;
                if (updateTicketDTO.TicketOwner.Name != null) existingTicket.Owner.Name = updateTicketDTO.TicketOwner.Name;
                if (updateTicketDTO.TicketOwner.Surname != null) existingTicket.Owner.Surname = updateTicketDTO.TicketOwner.Surname;
                if (updateTicketDTO.TicketOwner.Phone != null) existingTicket.Owner.Phone = updateTicketDTO.TicketOwner.Phone;
                if (updateTicketDTO.TicketOwner.Email != null) existingTicket.Owner.Email = updateTicketDTO.TicketOwner.Email;
            }

            //_mapper.Map(updateTicketDTO, existingTicket);
            var updated = await _ticketRepository.UpdateAsync(existingTicket);
            //var dto = _mapper.Map<TicketDto>(updated);

            string cacheKey = $"ticket:{updated.Id}";
            await _cache.RemoveAsync(cacheKey);

            return updated;
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

        public async Task UpdateConfirmationAsync(ConfirmedObjectDto confirmedObjectDto)
        {
            var objId = confirmedObjectDto.ObjId;
            var foundTicket = await _ticketRepository.GetByIdAsync(objId);
            if (foundTicket != null)
            {
                foundTicket.ConfirmStatus = ConfirmStatus.CONFFIRMED;
                foundTicket.ConfirmedAt = confirmedObjectDto.ConfirmDateTime;
                foundTicket.ConfirmatorId = confirmedObjectDto.ConfirmatorId;

                await _ticketRepository.UpdateAsync(foundTicket);
            }
        }
    }
}
