using MS_Lab.dto.ticket;

namespace MS_Lab.services.tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDTO>> GetAllTicketsAsync();
        Task<TicketDTO> GetTicketByIdAsync(string id);
        Task<TicketDTO> CreateTicketAsync(CreateTicketDTO createTicketDTO);
        Task<TicketDTO> UpdateTicketAsync(string id, UpdateTicketDTO updateTicketDTO);
        Task DeleteTicketAsync(string id);
    }
}
