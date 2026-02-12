using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using MS_Lab.repositories;
using MS_Lab.dto.ticket;
using MS_Lab.exception;
using MS_Lab.entities;

namespace MS_Lab.services.tickets
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;

        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<TicketDTO>>(tickets);
        }

        public async Task<TicketDTO> GetTicketByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                throw NotFoundException($"Билет с id={id} не найден");
            }

            return _mapper.Map<TicketDTO>(ticket);
        }

        public async Task<TicketDTO> CreateTicketAsync(CreateTicketDTO createTicketDTO)
        {
            int eventId = createTicketDTO.EventId;
            ValidateEvent(eventId);

            Ticket ticket = _mapper.Map<Ticket>(createTicketDTO);
            var savedTicket = await _ticketRepository.CreateAsync(ticket);

            return _mapper.Map<TicketDTO>(savedTicket);
        }

        public async Task<TicketDTO> UpdateTicketAsync(int ticketId, UpdateTicketDTO updateTicketDTO)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                throw NotFoundException($"Билет с id={ticketId} не найден");
            }

            int updatedEventId = updateTicketDTO.EventId;
            ValidateEvent(updatedEventId);

            ticket = _mapper.Map<Ticket>(updateTicketDTO);
            ticket.Id = ticketId;

            var updatedTicket = await _ticketRepository.UpdateAsync(ticket);
            return _mapper.Map<TicketDTO>(updatedTicket);
        }

        public async Task DeleteTicketAsync(int id)
        {
            if (!await _ticketRepository.ExistsByIdAsync(id))
            {
                throw NotFoundException($"Билет с id={id} не найден");
            }

            await _ticketRepository.DeleteAsync(id);
        }

        private async void ValidateEvent(int eventId)
        {
            var foundEvent = await _eventRepository.GetByIdAsync(eventId);
            if (foundEvent == null)
            {
                throw new NotFoundException($"Событие с id={eventId} не найдено");
            }

            int soldTicketNumber = await _ticketRepository.GetSoldTicketNumberByEventIdAsync(eventId);
            if (soldTicketNumber == foundEvent.TicketCount)
            {
                throw new BadRequestException("Все билеты проданы");
            }
        }
    }
}
