using AutoMapper;
using MS_Lab.dto.ticket;
using MS_Lab.exception;
using MS_Lab.entities;
using MS_Lab.repositories.events;
using MS_Lab.repositories.tickets;
using Microsoft.Extensions.Logging;
using MS_Lab.specification;

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

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync(TicketFilterDto filter)
        {
            var spec = TicketSpecification.FromFilter(filter);

            var tickets = await _ticketRepository.GetAllAsync(spec);

            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }

        public async Task<TicketDto> GetTicketByIdAsync(string id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                throw new NotFoundException($"Билет с id={id} не найден");
            }

            return _mapper.Map<TicketDto>(ticket);
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

            Ticket ticket = _mapper.Map<Ticket>(createTicketDTO);
            ticket.Event = foundEvent;

            var savedTicket = await _ticketRepository.CreateAsync(ticket);

            return _mapper.Map<TicketDto>(savedTicket);
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
            return _mapper.Map<TicketDto>(updated);
        }

        public async Task DeleteTicketAsync(string id)
        {
            if (!await _ticketRepository.ExistsByIdAsync(id))
            {
                throw new NotFoundException($"Билет с id={id} не найден");
            }

            await _ticketRepository.DeleteAsync(id);
        }
    }
}
