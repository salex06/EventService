using MS_Lab.entities;

namespace MS_Lab.dto.ticket
{
    public record TicketDto
    {
        public string Id { get; init; } = string.Empty;
        public string EventId { get; init; } = string.Empty;
        public string TicketNumber { get; init; } = string.Empty;
        public DateTime PurchaseDate { get; init; }
        public TicketOwnerDto? TicketOwner { get; init; }
        public ConfirmStatus ConfirmStatus { get; init; }
        public DateTime? ConfirmedAt { get; init; }
    }
}
