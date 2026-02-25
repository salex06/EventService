using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public class EventFilterDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Place { get; set; }
        public EventType? EventType { get; set; }
        public DateTime? MinStartTimeUTC { get; set; }
        public DateTime? MaxStartTimeUTC { get; set; }
        public DateTime? MinEndTimeUTC { get; set; }
        public DateTime? MaxEndTimeUTC { get; set; }
        public int? MinTicketCount { get; set; }
        public int? MaxTicketCount { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public bool HasFilters() {
            return !string.IsNullOrWhiteSpace(Name)
                || !string.IsNullOrWhiteSpace(Description)
                || !string.IsNullOrWhiteSpace(Place)
                || EventType.HasValue
                || MinStartTimeUTC.HasValue
                || MaxStartTimeUTC.HasValue
                || MinEndTimeUTC.HasValue
                || MaxEndTimeUTC.HasValue
                || MinTicketCount.HasValue
                || MaxTicketCount.HasValue
                || MinPrice.HasValue
                || MaxPrice.HasValue;
        }
    }
}
