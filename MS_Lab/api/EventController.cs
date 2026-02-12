using Microsoft.AspNetCore.Mvc;
using MS_Lab.dto.events;
using MS_Lab.services.events;

namespace MS_Lab.api
{
    [ApiController]
    [Route("api/event")]
    public class EventController : Controller
    {

        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetAll() {
            var events = await _eventService.GetAllEventsAsync();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetById(int id) {
            var foundEvent = await _eventService.GetEventByIdAsync(id);

            return Ok(foundEvent);
        }

        [HttpPost]
        public async Task<ActionResult<EventDTO>> Create(CreateEventDTO eventInfo) {
            var createdEvent = await _eventService.CreateEventAsync(eventInfo);

            return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id}, createdEvent);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<EventDTO>> Update(int id, UpdateEventDTO eventInfo)
        {
            var updatedEvent = await _eventService.UpdateEventAsync(id, eventInfo);
            return Ok(updatedEvent);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id) {
            await _eventService.DeleteEventAsync(id);

            return Ok();
        }

    }
}
