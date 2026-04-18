using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MS_Lab.dto;
using MS_Lab.dto.events;
using MS_Lab.dto.ticket;
using MS_Lab.filter;
using MS_Lab.kafka.producer;
using MS_Lab.services.tickets;
using System.Text.Json;

namespace MS_Lab.api
{
    [ApiController]
    [Route("api/ticket")]
    public class TicketController : ControllerBase
    {

        private readonly ITicketService _ticketService;
        private readonly IKafkaMessageProducer _kafkaMessageProducer;
        private readonly ProducerSettings _producerSettings;
        public TicketController(
            ITicketService ticketService, 
            IKafkaMessageProducer producer,
            IOptions<ProducerSettings> producerSettings)
        {
            _ticketService = ticketService;
            _kafkaMessageProducer = producer;
            _producerSettings = producerSettings.Value;

        }

        /// <summary>
        /// Получить все билеты
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/ticket
        /// 
        /// </remarks>
        /// <returns>Возвращает список всех билетов в системе</returns>
        /// <response code="200">Список билетов успешно получен</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAll([FromQuery] TicketFilterDto filter)
        {
            var tickets = await _ticketService.GetAllTicketsAsync(filter);

            return Ok(tickets);
        }

        /// <summary>
        /// Получить билет по ID
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/ticket/djvhuv3nc2u389371d
        /// 
        /// </remarks>
        /// <param name="id">ID билета</param>
        /// <returns>Детальная информация о билете</returns>
        /// <response code="200">Билет найден</response>
        /// <response code="404">Билет не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<TicketDto>> GetById(string id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        /// <summary>
        /// Создать новый билет
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     POST /api/ticket
        ///     {
        ///     "eventId": 7,
        ///     "ticketOwner": {
        ///         "name": "Иван",
        ///         "surname": "Петров",
        ///         "phone": "+79001234567",
        ///         "email": "ivan.petrov@email.com"
        ///     }
        /// </remarks>
        /// <param name="ticketInfo">Данные для создания билета</param>
        /// <returns>Детальная информация о билете</returns>
        /// <response code="201">Билет создан</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Событие не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        [ProducesResponseType(typeof(TicketDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<TicketDto>> Create(CreateTicketDto ticketInfo)
        {
            var ticket = await _ticketService.CreateTicketAsync(ticketInfo);

            SendConfirmationRequest(ticket, ticketInfo.ConfirmatorId);

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        /// <summary>
        /// Обновить данные о билете
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     PATCH /api/ticket/ccfurwldnc21
        ///     {
        ///     "eventId": 7,
        ///     "ticketOwner": {
        ///         "name": "Иван",
        ///         "surname": "Петров",
        ///         "phone": "+79001234567",
        ///         "email": "ivan.petrov@email.com"
        ///     }
        /// </remarks>
        /// <param name="ticketInfo">Новые данные</param>
        /// <returns>Детальная информация о билете</returns>
        /// <response code="200">Билет отредактирован</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Билет не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(TicketDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<TicketDto>> Update(string id, UpdateTicketDto ticketInfo)
        {
            var ticket = await _ticketService.UpdateTicketAsync(id, ticketInfo);

            return Ok(ticket);
        }

        /// <summary>
        /// Удалить билет
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///     DELETE /api/ticket/22od393d38dnc
        ///     
        /// </remarks>
        /// <response code="200">Билет удалён</response>
        /// <response code="404">Билет не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> Delete(string id)
        {
            await _ticketService.DeleteTicketAsync(id);

            return Ok();
        }


        private void SendConfirmationRequest(TicketDto createdTicket, string confirmatorId)
        {
            RegObjectDto regObject = new RegObjectDto()
            {
                Type = ObjectType.Ticket,
                ObjectId = createdTicket.Id,
                ConfirmatorId = confirmatorId
            };
            string message = JsonSerializer.Serialize(regObject);

            _kafkaMessageProducer.SendMessageAsync(_producerSettings.TopicName, message);
        }

    }
}
