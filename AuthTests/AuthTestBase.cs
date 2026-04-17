using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace GatewayTests
{
    public abstract class GatewayTestBase
    {
        protected readonly HttpClient _client;
        protected string? _validToken;
        protected string _username = "admin";
        protected string _password = "admin123";

        // Секретный ключ должен совпадать с ключом в AuthService и шлюзе
        private const string JwtSecret = "SuperSecretKeyForJwtTokenGeneration123";

        protected GatewayTestBase()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5001")
            };
        }

        protected async Task RegisterAdmin()
        {
            var registerDto = new { Username = _username, Password = _password, Role = "Admin" };
            var content = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/auth/register", content);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register user: {response.StatusCode} - {error}");
            }
        }

        protected async Task<string> GetValidTokenAsync()
        {
            if (_validToken != null)
                return _validToken;

            await RegisterAdmin();
            var loginDto = new { Username = _username, Password = _password };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/auth/login", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            _validToken = tokenResponse!["token"];
            return _validToken;
        }

        // Генерация просроченного токена
        protected string GetExpiredToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            }),
                Expires = DateTime.UtcNow.AddSeconds(1),
                Issuer = "AuthService",
                Audience = "ApiGateway",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            Thread.Sleep(1222);
            return tokenHandler.WriteToken(token);
        }

        // Генерация токена с неверной подписью (используем другой ключ)
        protected string GetInvalidSignatureToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var fakeKey = Encoding.UTF8.GetBytes("WrongSecretKeyForJwtTokenGeneration");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "admin"),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "AuthService",
                Audience = "ApiGateway",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(fakeKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}