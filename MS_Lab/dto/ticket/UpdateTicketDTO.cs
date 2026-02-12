namespace MS_Lab.dto.ticket
{
    public record UpdateTicketDTO
    {
        public int EventId { get; init; }
        public TicketOwnerDTO? TicketOwner { get; init; }
    }
}
