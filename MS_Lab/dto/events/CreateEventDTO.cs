using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public record CreateEventDTO
    {
        public required string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public required string Place { get; init; } = string.Empty;
        public EventType EventType { get; init; }
        public required DateTime StartTimeUTC { get; init; }
        public required DateTime EndTimeUTC { get; init; }
        public required int TicketCount { get; init; }
        public required int Price { get; init; }
    }
}
