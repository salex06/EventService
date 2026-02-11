using MS_Lab.enums;

namespace MS_Lab.dto.events
{
    public record UpdateEventDTO
    {
        int Id;
        string? Name;
        string? Description;
        string? Place;
        EventType? EventType;

        DateTime? StartTimeUTC;
        DateTime? EndTimeUTC;

        int? TicketCount;
        int? Price;
    }
}
