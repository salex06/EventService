using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Driver;
using MS_Lab.dto.events;
using MS_Lab.entities;
using MS_Lab.enums;
using MS_Lab.services.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MS_Lab.Tests.Integration
{

    /// <summary>
    /// Интеграционные тесты для EventController.
    /// Используют выделенную тестовую БД MongoDB.
    /// </summary>
    public class EventApiTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly IMongoCollection<Event> _eventCollection;
        private const string TestDatabaseName = "TestMongoDb";

        public EventApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["MongoDb:DatabaseName"] = TestDatabaseName,
                        ["MongoDb:ConnectionString"] = "mongodb://root:password@localhost:27017",
                        ["RepositorySettings:ObjectPerRequestLimit"] = "200000"
                    }!);
                });
            }).CreateClient();

            try
            {
                var mongoClient = new MongoClient("mongodb://root:password@localhost:27017");
                _eventCollection = mongoClient.GetDatabase(TestDatabaseName).GetCollection<Event>("events");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к MongoDB: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Перед каждым тестом очищаем коллекцию событий.
        /// </summary>
        public async Task InitializeAsync()
        {
            await _eventCollection.DeleteManyAsync(FilterDefinition<Event>.Empty);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        /// <summary>
        /// Тест добавления 100 событий через API.
        /// </summary>
        [Fact]
        public async Task Add100Events_ThroughApi_ShouldIncreaseCountBy100()
        {
            const int count = 100;
            var events = GenerateCreateEventDtos(count);

            await AddEventsConcurrently(events);

            var response = await _client.GetAsync("/api/event");
            response.EnsureSuccessStatusCode();
            var eventsDto = await response.Content.ReadFromJsonAsync<List<EventDto>>();
            Assert.Equal(count, eventsDto!.Count);
        }

        /// <summary>
        /// Тест добавления 100 000 событий через API.
        /// </summary>
        [Fact]
        public async Task Add100000Events_ThroughApi_ShouldIncreaseCountBy100000()
        {
            const int count = 100_000;
            var events = GenerateCreateEventDtos(count);

            await AddEventsConcurrently(events);

            var response = await _client.GetAsync("/api/event");
            response.EnsureSuccessStatusCode();
            var eventsDto = await response.Content.ReadFromJsonAsync<List<EventDto>>();
            Assert.Equal(count, eventsDto!.Count);
        }

        /// <summary>
        /// Тест удаления всех событий через API.
        /// Добавляем 100 событий, затем удаляем их по одному.
        /// </summary>
        [Fact]
        public async Task DeleteAllEvents_ThroughApi_ShouldMakeListEmpty()
        {
            const int count = 100;
            var events = GenerateCreateEventDtos(count);
            await AddEventsConcurrently(events);

            var response = await _client.GetAsync("/api/event");
            response.EnsureSuccessStatusCode();
            var eventsDto = await response.Content.ReadFromJsonAsync<List<EventDto>>();
            var ids = eventsDto!.Select(e => e.Id).ToList();

            await DeleteEventsConcurrently(ids);

            response = await _client.GetAsync("/api/event");
            response.EnsureSuccessStatusCode();
            eventsDto = await response.Content.ReadFromJsonAsync<List<EventDto>>();
            Assert.Empty(eventsDto!);
        }

        /// <summary>
        /// Генерация заданного количества DTO для создания событий.
        /// </summary>
        private static List<CreateEventDto> GenerateCreateEventDtos(int count)
        {
            var list = new List<CreateEventDto>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new CreateEventDto
                {
                    Name = $"Test Event {i}",
                    Description = $"Description {i}",
                    Place = $"Place {i}",
                    EventType = EventType.Conference,
                    StartTimeUTC = DateTime.UtcNow.AddDays(1),
                    EndTimeUTC = DateTime.UtcNow.AddDays(1).AddHours(2),
                    TicketCount = 100,
                    Price = 50
                });
            }
            return list;
        }

        /// <summary>
        /// Конкурентное добавление событий через POST /api/event.
        /// </summary>
        private async Task AddEventsConcurrently(List<CreateEventDto> events, int maxParallelism = 50)
        {
            using var semaphore = new SemaphoreSlim(maxParallelism);
            var tasks = events.Select(async e =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var response = await _client.PostAsJsonAsync("/api/event", e);
                    response.EnsureSuccessStatusCode();
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Конкурентное удаление событий через DELETE /api/event/{id}.
        /// </summary>
        private async Task DeleteEventsConcurrently(List<string> ids, int maxParallelism = 50)
        {
            using var semaphore = new SemaphoreSlim(maxParallelism);
            var tasks = ids.Select(async id =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var response = await _client.DeleteAsync($"/api/event/{id}");
                    response.EnsureSuccessStatusCode();
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(tasks);
        }
    }
}