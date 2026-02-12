namespace MS_Lab.dto.ticket
{
    public record TicketDTO
    {
        public int Id { get; init; }
        public int EventId { get; init; }
        public TicketOwnerDTO? TicketOwner { get; init; }
    }
}
