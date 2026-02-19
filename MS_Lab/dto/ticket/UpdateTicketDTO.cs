namespace MS_Lab.dto.ticket
{
    public record UpdateTicketDTO
    {
        //public string EventId { get; init; } = string.Empty;
        public TicketOwnerDTO? TicketOwner { get; init; }
    }
}
