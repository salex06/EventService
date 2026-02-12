namespace MS_Lab.dto.ticket
{
    public record CreateTicketDTO
    {
        public int EventId { get; init; }
        public TicketOwnerDTO? TicketOwner { get; init; }
    }
}
