namespace MS_Lab.dto.ticket
{
    public record TicketDTO
    {
        int Id;
        int EventId;

        TicketOwnerDTO TicketOwner;

        public TicketDTO(int id, int eventId, TicketOwnerDTO ticketOwner)
        {
            Id = id;
            EventId = eventId;
            TicketOwner = ticketOwner;
        }
    }
}
