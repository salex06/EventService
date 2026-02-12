namespace MS_Lab.dto.ticket
{
    public record UpdateTicketDTO
    {
        public int EventId;

        public TicketOwnerDTO TicketOwner;

        public UpdateTicketDTO(int eventId, TicketOwnerDTO ticketOwner)
        {
            EventId = eventId;
            TicketOwner = ticketOwner;
        }
    }
}
