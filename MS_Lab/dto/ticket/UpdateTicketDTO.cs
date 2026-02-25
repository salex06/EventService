namespace MS_Lab.dto.ticket
{
    public record UpdateTicketDto
    {
        public TicketOwnerDto? TicketOwner { get; init; }
    }
}
