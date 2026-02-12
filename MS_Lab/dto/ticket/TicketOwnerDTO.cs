namespace MS_Lab.dto.ticket
{
    public record TicketOwnerDTO
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Surname { get; init; } = string.Empty;
        public string? Phone { get; init; }
        public string Email { get; init; } = string.Empty;
    }
}
