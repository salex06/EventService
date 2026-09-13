using MS_Lab.dto;
using MS_Lab.dto.ticket;
using MS_Lab.entities;

namespace MS_Lab.services.tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync(TicketFilterDto filter);
        Task<Ticket> GetTicketByIdAsync(string id);
        Task<Ticket> CreateTicketAsync(CreateTicketDto createTicketDTO);
        Task<Ticket> UpdateTicketAsync(string id, UpdateTicketDto updateTicketDTO);
        Task DeleteTicketAsync(string id);
        Task UpdateConfirmationAsync(ConfirmedObjectDto confirmedObjectDto);
    }
}
