using Microsoft.AspNetCore.Mvc;
using MS_Lab.dto.events;

namespace MS_Lab.api
{
    [ApiController]
    [Route("api/event")]
    public class EventController : Controller
    {

        private readonly EventService _eventService;

        public EventController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetAll() {
            var events = await _eventService.getAllEvents();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetById() {
            var foundEvent = await _eventService.getEventById();

            return Ok(foundEvent);
        }

        [HttpPost]
        public async Task<ActionResult<EventDTO>> Create(CreateEventDTO eventInfo) {
            var createdEvent = await _eventService.createEvent();

            return Ok(createdEvent);
        }

        [HttpPatch]
        public async Task<ActionResult<EventDTO>> Update(UpdateEventDTO eventInfo) {
            var updatedEvent = await _eventService.updateEvent();

            return Ok(updatedEvent);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete() {
            await _eventService.deleteEvent();

            return Ok();
        }

    }
}
