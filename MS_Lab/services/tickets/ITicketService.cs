using MS_Lab.dto.ticket;

namespace MS_Lab.services.tickets
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
        Task<TicketDto> GetTicketByIdAsync(string id);
        Task<TicketDto> CreateTicketAsync(CreateTicketDto createTicketDTO);
        Task<TicketDto> UpdateTicketAsync(string id, UpdateTicketDto updateTicketDTO);
        Task DeleteTicketAsync(string id);
    }
}
