using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace GatewayTests
{
    public class EventsApiAuthTests : GatewayTestBase
    {
        [Fact]
        public async Task GetEvents_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/event");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetEvents_WithValidToken_ReturnsOk()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync("/api/event");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetEvents_WithInvalidToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetInvalidSignatureToken());
            var response = await _client.GetAsync("/api/event");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GetEvents_WithExpiredToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetExpiredToken());
            var response = await _client.GetAsync("/api/event");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_WithoutToken_ReturnsUnauthorized()
        {
            var dto = new
            {
                Name = "Test Event",
                Description = "Desc",
                Place = "Place",
                EventType = 0,
                StartTimeUTC = DateTime.UtcNow.AddDays(1),
                EndTimeUTC = DateTime.UtcNow.AddDays(1).AddHours(2),
                TicketCount = 100,
                Price = 500,
                ConfirmatorId = "test"
            };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/event", content);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_WithValidToken_ReturnsCreated()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var dto = new
            {
                Name = "Test Event " + Guid.NewGuid(),
                Description = "Desc",
                Place = "Place",
                EventType = 0,
                StartTimeUTC = DateTime.UtcNow.AddDays(1),
                EndTimeUTC = DateTime.UtcNow.AddDays(1).AddHours(2),
                TicketCount = 100,
                Price = 500,
                ConfirmatorId = "test"
            };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/event", content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task DeleteEvent_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/event/some-id");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}