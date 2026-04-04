using System.Text.Json.Serialization;

namespace ClientService.dto
{
    public record UpdateUserDto
    {
        [JsonRequired]
        public string Id { get; init; } = string.Empty;
        
        [JsonRequired]
        public string Name { get; init; } = string.Empty;

        [JsonRequired]
        public string Email { get; init; } = string.Empty;
    }
}
