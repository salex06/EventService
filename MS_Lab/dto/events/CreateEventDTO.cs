using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public record CreateEventDTO
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Place { get; init; } = string.Empty;
        public EventType EventType { get; init; }
        public DateTime StartTimeUTC { get; init; }
        public DateTime EndTimeUTC { get; init; }
        public int TicketCount { get; init; }
        public int Price { get; init; }
    }
}
