using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace GatewayTests
{
    public class TicketsApiAuthTests : GatewayTestBase
    {
        [Fact]
        public async Task GetTickets_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/ticket");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetTickets_WithValidToken_ReturnsOk()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync("/api/ticket");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_WithoutToken_ReturnsUnauthorized()
        {
            var dto = new
            {
                EventId = "507f1f77bcf86cd799439011",
                TicketOwner = new { Name = "Ivan", Surname = "Petrov", Email = "i@example.com" },
                ConfirmatorId = "test"
            };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/ticket", content);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_WithInvalidToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetInvalidSignatureToken());
            var dto = new
            {
                EventId = "507f1f77bcf86cd799439011",
                TicketOwner = new { Name = "Ivan", Surname = "Petrov", Email = "i@example.com" },
                ConfirmatorId = "test"
            };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/ticket", content);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_WithExpiredToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetExpiredToken());
            var dto = new
            {
                EventId = "507f1f77bcf86cd799439011",
                TicketOwner = new { Name = "Ivan", Surname = "Petrov", Email = "i@example.com" },
                ConfirmatorId = "test"
            };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/ticket", content);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateTicket_WithValidToken_ReturnsCreated()
        {
            // Сначала создаём событие (можно сделать через API или предположить, что событие уже есть)
            // Для простоты можно создать событие отдельным запросом
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Создаём событие
            var eventDto = new
            {
                Name = "Event for Ticket " + Guid.NewGuid(),
                Description = "Desc",
                Place = "Place",
                EventType = 0,
                StartTimeUTC = DateTime.UtcNow.AddDays(1),
                EndTimeUTC = DateTime.UtcNow.AddDays(1).AddHours(2),
                TicketCount = 10,
                Price = 100,
                ConfirmatorId = "test"
            };
            var eventContent = new StringContent(JsonSerializer.Serialize(eventDto), Encoding.UTF8, "application/json");
            var eventResponse = await _client.PostAsync("/api/event", eventContent);
            Assert.Equal(HttpStatusCode.Created, eventResponse.StatusCode);
            var eventJson = await eventResponse.Content.ReadAsStringAsync();
            var createdEvent = JsonSerializer.Deserialize<JsonElement>(eventJson);
            var eventId = createdEvent.GetProperty("id").GetString();

            // Создаём билет
            var ticketDto = new
            {
                EventId = eventId,
                TicketOwner = new { Name = "Ivan", Surname = "Petrov", Email = "i@example.com" },
                ConfirmatorId = "test"
            };
            var ticketContent = new StringContent(JsonSerializer.Serialize(ticketDto), Encoding.UTF8, "application/json");
            var ticketResponse = await _client.PostAsync("/api/ticket", ticketContent);
            Assert.Equal(HttpStatusCode.Created, ticketResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteTicket_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync("/api/ticket/some-id");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}