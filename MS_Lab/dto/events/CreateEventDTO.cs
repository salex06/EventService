using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public record CreateEventDTO
    {
        string Name;
        string Description;
        string Place;
        EventType EventType;

        DateTime StartTimeUTC;
        DateTime EndTimeUTC;

        int TicketCount;
        int Price;

        public CreateEventDTO(string name, string description, string place, 
            EventType eventType, DateTime startTimeUTC, DateTime endTimeUTC, 
            int ticketCount, int price)
        {
            Name = name;
            Description = description;
            Place = place;
            EventType = eventType;
            StartTimeUTC = startTimeUTC;
            EndTimeUTC = endTimeUTC;
            TicketCount = ticketCount;
            Price = price;
        }
    }
}
