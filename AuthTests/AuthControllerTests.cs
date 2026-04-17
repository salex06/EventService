using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace GatewayTests
{
    public class AuthApiTests : GatewayTestBase
    {
        [Fact]
        public async Task Register_ValidData_ReturnsToken()
        {
            var uniqueUser = "testuser_" + Guid.NewGuid().ToString("N")[..8];
            var dto = new { Username = uniqueUser, Password = "Pass123!", Role = "User" };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/register", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.True(result.ContainsKey("token"));
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Сначала регистрируем пользователя
            var user = "loginuser_" + Guid.NewGuid().ToString("N")[..8];
            var pass = "Pass123!";
            await _client.PostAsync("/api/auth/register", new StringContent(JsonSerializer.Serialize(new { Username = user, Password = pass }), Encoding.UTF8, "application/json"));

            var loginDto = new { Username = user, Password = pass };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var loginDto = new { Username = "nonexistent", Password = "wrong" };
            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}