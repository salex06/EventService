using AutoMapper;
using MS_Lab.dto.ticket;
using MS_Lab.exception;
using MS_Lab.entities;
using MS_Lab.repositories.events;
using MS_Lab.repositories.tickets;
using Microsoft.Extensions.Logging;

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

        public async Task<TicketDTO> GetTicketByIdAsync(string id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                throw new NotFoundException($"Билет с id={id} не найден");
            }

            return _mapper.Map<TicketDTO>(ticket);
        }

        public async Task<TicketDTO> CreateTicketAsync(CreateTicketDTO createTicketDTO)
        {
            string eventId = createTicketDTO.EventId;
            await ValidateEventAsync(eventId);

            Ticket ticket = _mapper.Map<Ticket>(createTicketDTO);
            var savedTicket = await _ticketRepository.CreateAsync(ticket);

            return _mapper.Map<TicketDTO>(savedTicket);
        }

        public async Task<TicketDTO> UpdateTicketAsync(string ticketId, UpdateTicketDTO updateTicketDTO)
        {
            var existingTicket = await _ticketRepository.GetByIdAsync(ticketId);
            if (existingTicket == null)
                throw new NotFoundException($"Билет с id={ticketId} не найден");

            var foundEvent = await _eventRepository.GetByIdAsync(updateTicketDTO.EventId);
            if (foundEvent == null)
                throw new NotFoundException($"Событие с id={updateTicketDTO.EventId} не найдено");

            _mapper.Map(updateTicketDTO, existingTicket);
            var updated = await _ticketRepository.UpdateAsync(existingTicket);
            return _mapper.Map<TicketDTO>(updated);
        }

        public async Task DeleteTicketAsync(string id)
        {
            if (!await _ticketRepository.ExistsByIdAsync(id))
            {
                throw new NotFoundException($"Билет с id={id} не найден");
            }

            await _ticketRepository.DeleteAsync(id);
        }

        private async Task ValidateEventAsync(string eventId)
        {
            var foundEvent = await _eventRepository.GetByIdAsync(eventId);
            if (foundEvent == null)
                throw new NotFoundException($"Событие с id={eventId} не найдено");

            var soldTicketNumber = await _ticketRepository.GetSoldTicketNumberByEventIdAsync(eventId);
            if (soldTicketNumber == foundEvent.TicketCount)
                throw new BadRequestException("Все билеты проданы");
        }
    }
}
