using MS_Lab.enums;
using System.Text.Json.Serialization;

namespace MS_Lab.dto.events
{
    public record CreateEventDto
    {
        [JsonRequired]
        public string Name { get; init; } = string.Empty;

        [JsonRequired]
        public string Description { get; init; } = string.Empty;

        [JsonRequired]
        public string Place { get; init; } = string.Empty;

        [JsonRequired]
        public EventType EventType { get; init; }

        [JsonRequired]
        public DateTime StartTimeUTC { get; init; }

        [JsonRequired]
        public DateTime EndTimeUTC { get; init; }

        [JsonRequired]
        public int TicketCount { get; init; }

        [JsonRequired]
        public int Price { get; init; }
    }
}
