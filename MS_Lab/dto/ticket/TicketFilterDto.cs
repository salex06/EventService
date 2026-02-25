namespace MS_Lab.dto.ticket
{
    public class TicketFilterDto
    {
        public string? TicketNumber { get; set; }
        public DateTime? MinPurchaseDate { get; set; }
        public DateTime? MaxPurchaseDate { get; set; }

        public bool HasFilters() {
            return !string.IsNullOrWhiteSpace(TicketNumber)
                || MinPurchaseDate.HasValue
                || MaxPurchaseDate.HasValue;
        }
    }
}
