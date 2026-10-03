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

            ArgumentNullException.ThrowIfNull(createTicketDTO);

            var ticketOwner = createTicketDTO.TicketOwner
                ?? throw new BadRequestException("Требуется заполнить поля владельца билета");

            var owner = new TicketOwner
            {
                Id = ticketOwner.Id ?? throw new BadRequestException("Id обязателен"),
                Name = ticketOwner.Name ?? throw new BadRequestException("Name обязателен"),
                Surname = ticketOwner.Surname ?? throw new BadRequestException("Surname обязателен"),
                Phone = ticketOwner.Phone ?? throw new BadRequestException("Phone обязателен"),
                Email = ticketOwner.Email ?? throw new BadRequestException("Email обязателен"),
            };

            Ticket ticket = new Ticket
            {
                Event = foundEvent,
                Owner = owner,
                ConfirmatorId = createTicketDTO.ConfirmatorId
            };

            var savedTicket = await _ticketRepository.CreateAsync(ticket);

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

            var updated = await _ticketRepository.UpdateAsync(existingTicket);
            if (updated == null)
                throw new BadRequestException("Ошибка обновления данных билета");

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
