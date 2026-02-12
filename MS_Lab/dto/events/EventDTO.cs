using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public record EventDTO
    {
        public int Id;
        public string Name;
        public string Description;
        public string Place;
        public EventType EventType;

        public DateTime StartTimeUTC;
        public DateTime EndTimeUTC;

        public int TicketCount;
        public int Price;

        public EventDTO(int id, string name, string description, string place, 
            EventType eventType, DateTime startTimeUTC, DateTime endTimeUTC, 
            int ticketCount, int price)
        {
            Id = id;
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
