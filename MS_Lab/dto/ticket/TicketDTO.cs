namespace MS_Lab.dto.ticket
{
    public record TicketDTO
    {
        public string Id { get; init; } = string.Empty;
        public string EventId { get; init; } = string.Empty;
        public string TicketNumber { get; init; } = string.Empty;
        public TicketOwnerDTO? TicketOwner { get; init; }
    }
}
