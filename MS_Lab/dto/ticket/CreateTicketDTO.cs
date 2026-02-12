namespace MS_Lab.dto.ticket
{
    public record CreateTicketDTO
    {
        public int EventId;

        public TicketOwnerDTO TicketOwner;

        public CreateTicketDTO(int eventId, TicketOwnerDTO ticketOwner)
        {
            EventId = eventId;
            TicketOwner = ticketOwner;
        }
    }
}
