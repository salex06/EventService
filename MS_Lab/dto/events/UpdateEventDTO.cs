using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public record UpdateEventDTO
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public string? Place { get; init; }
        public EventType? EventType { get; init; }
        public DateTime? StartTimeUTC { get; init; }
        public DateTime? EndTimeUTC { get; init; }
        public int? TicketCount { get; init; }
        public int? Price { get; init; }
    }
}
