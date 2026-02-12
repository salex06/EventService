using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public record CreateEventDTO
    {
        public string Name;
        public string Description;
        public string Place;
        public EventType EventType;
         
        public DateTime StartTimeUTC;
        public DateTime EndTimeUTC;

        public int TicketCount;
        public int Price;

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
