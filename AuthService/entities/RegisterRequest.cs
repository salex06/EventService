namespace AuthService.Models
{
    public record RegisterRequest(string Username, string Password, string? Role = "User");
}