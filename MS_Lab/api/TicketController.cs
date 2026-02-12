using Microsoft.AspNetCore.Mvc;
using MS_Lab.dto.ticket;
using MS_Lab.services.tickets;

namespace MS_Lab.api
{
    [ApiController]
    [Route("api/ticket")]
    public class TicketController : Controller
    {

        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAll() {
            var users = await _ticketService.GetAllTicketsAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<TicketDTO>> GetById(int id) {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null) {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketDTO>> Create(CreateTicketDTO ticketInfo) {
            var ticket = await _ticketService.CreateTicketAsync(ticketInfo);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<TicketDTO>> Update(int id, UpdateTicketDTO ticketInfo) {
            var ticket = await _ticketService.UpdateTicketAsync(id, ticketInfo);

            return Ok(ticket);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id) {
            await _ticketService.DeleteTicketAsync(id);

            return Ok();
        }

    }
}
