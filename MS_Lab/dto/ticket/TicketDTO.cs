namespace MS_Lab.dto.ticket
{
    public record TicketDto
    {
        public string Id { get; init; } = string.Empty;
        public string EventId { get; init; } = string.Empty;
        public string TicketNumber { get; init; } = string.Empty;
        public TicketOwnerDto? TicketOwner { get; init; }
    }
}
