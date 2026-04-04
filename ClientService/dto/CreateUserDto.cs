using System.Text.Json.Serialization;

namespace ClientService.dto
{
    public record CreateUserDto
    {
        [JsonRequired]
        public string Name { get; init; } = string.Empty;

        [JsonRequired]
        public string Email { get; init; } = string.Empty;
    }
}
