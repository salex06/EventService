namespace MS_Lab.dto.ticket
{
    public record CreateTicketDto
    {
        public string EventId { get; init; } = string.Empty;
        public TicketOwnerDto? TicketOwner { get; init; }
    }
}
