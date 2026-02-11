namespace MS_Lab.dto.ticket
{
    public record CreateTicketDTO
    {
        int EventId;

        TicketOwnerDTO TicketOwner;

        public CreateTicketDTO(int eventId, TicketOwnerDTO ticketOwner)
        {
            EventId = eventId;
            TicketOwner = ticketOwner;
        }
    }
}
