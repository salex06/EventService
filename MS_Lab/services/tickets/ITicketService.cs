using MS_Lab.dto.ticket;

namespace MS_Lab.services.tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDTO>> GetAllTicketsAsync();
        Task<TicketDTO> GetTicketByIdAsync(int id);
        Task<TicketDTO> CreateTicketAsync(CreateTicketDTO createTicketDTO);
        Task<TicketDTO> UpdateTicketAsync(int id, UpdateTicketDTO updateTicketDTO);
        Task DeleteTicketAsync(int id);
    }
}
