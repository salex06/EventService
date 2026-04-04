using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MS_Lab.dto;
using MS_Lab.dto.events;
using MS_Lab.entities;
using MS_Lab.filter;
using MS_Lab.kafka.producer;
using MS_Lab.services.events;
using System.Text.Json;

namespace MS_Lab.api
{
    [ApiController]
    [Route("api/event")]
    public class EventController : ControllerBase
    {

        private readonly IEventService _eventService;
        private readonly IKafkaMessageProducer _kafkaMessageProducer;
        private readonly ProducerSettings _producerSettings;
        public EventController(IEventService eventService, 
                               IKafkaMessageProducer kafkaMessageProducer,
                               IOptions<ProducerSettings> producerSettings)
        {
            _eventService = eventService;
            _kafkaMessageProducer = kafkaMessageProducer;
            _producerSettings = producerSettings.Value;
        }

        /// <summary>
        /// Получить все события
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/event
        /// 
        /// </remarks>
        /// <returns>Возвращает список всех событий в системе</returns>
        /// <response code="200">Список событий успешно получен</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAll([FromQuery] EventFilterDto filter)
        {
            var events = await _eventService.GetAllEventsAsync(filter);

            return Ok(events);
        }

        /// <summary>
        /// Получить событие по ID
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/event/af84kcn47fnds
        /// 
        /// </remarks>
        /// <param name="id">ID события (целое число)</param>
        /// <returns>Детальная информация о событии</returns>
        /// <response code="200">Событие найдено</response>
        /// <response code="404">Событие не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<EventDto>> GetById(string id)
        {
            var foundEvent = await _eventService.GetEventByIdAsync(id);

            return Ok(foundEvent);
        }

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// POST /api/event
        /// {
        ///    "name": "[Top Secret]",
        ///    "place": "Очень секретная конференция по использованию Postman",
        ///    "eventType": 1,
        ///    "startTimeUTC": "2026-01-06T21:00:00Z",
        ///    "endTimeUTC": "2026-01-06T22:00:00Z",
        ///    "ticketCount": 3,
        ///    "price": 100
        /// }
        /// 
        /// </remarks>
        /// <param name="eventInfo">Информация о событии</param>
        /// <returns>Детальная информация о событии</returns>
        /// <response code="201">Событие создано</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        [ProducesResponseType(typeof(EventDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<EventDto>> Create(CreateEventDto eventInfo)
        {
            var createdEvent = await _eventService.CreateEventAsync(eventInfo);

            SendConfirmationRequest(createdEvent, eventInfo.ConfirmatorId);

            return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
        }

        /// <summary>
        /// Редактировать данные события
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// PATCH /api/event/afe937dncle83jd31209
        /// {
        ///    "name": "[Top Secret]",
        ///    "place": "Очень секретная конференция по использованию Postman",
        ///    "eventType": 1,
        ///    "startTimeUTC": "2026-01-06T21:00:00Z",
        ///    "endTimeUTC": "2026-01-06T22:00:00Z",
        ///    "ticketCount": 3,
        ///    "price": 100
        /// }
        /// 
        /// </remarks>
        /// <param name="eventInfo">Информация о событии</param>
        /// <returns>Детальная информация о событии</returns>
        /// <response code="200">Данные изменены</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Событие не найдено данные</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<EventDto>> Update(string id, UpdateEventDto eventInfo)
        {
            var updatedEvent = await _eventService.UpdateEventAsync(id, eventInfo);
            return Ok(updatedEvent);
        }

        /// <summary>
        /// Удалить событие
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///     DELETE /api/event/5fbmvu104uyd83
        ///     
        /// </remarks>
        /// <response code="200">Событие удалено</response>
        /// <response code="404">Событие не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> Delete(string id)
        {
            await _eventService.DeleteEventAsync(id);

            return Ok();
        }


        [HttpGet("danger")]
        public ActionResult Get500() {
            return StatusCode(500);
        }

        [HttpGet("want_sleep")]
        public ActionResult WaitABit() {
            Thread.Sleep(2000);
            return Ok();
        }

        private void SendConfirmationRequest(EventDto createdEvent, string confirmatorId) {
            RegObjectDto regObject = new RegObjectDto()
            {
                Type = ObjectType.Event,
                ObjectId = createdEvent.Id,
                ConfirmatorId = confirmatorId
            };

            string message = JsonSerializer.Serialize(regObject);

            _kafkaMessageProducer.SendMessageAsync(_producerSettings.TopicName, message);
        }
    }
}
