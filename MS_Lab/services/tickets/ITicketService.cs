using MS_Lab.dto.ticket;

namespace MS_Lab.services.tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDTO>> GetAllTickets();
        Task<TicketDTO> GetTicketById(int id);
        Task<TicketDTO> CreateTicket(CreateTicketDTO createTicketDTO);
        Task<TicketDTO> UpdateTicket(UpdateTicketDTO updateTicketDTO);
        Task DeleteTicket(int id);
    }
}
