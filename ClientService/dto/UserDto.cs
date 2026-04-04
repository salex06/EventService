namespace ClientService.dto
{
    public record UserDto
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;

        public int RegisteredObjects { get; init; }
    }
}
