using Microsoft.AspNetCore.Mvc;

namespace MS_Lab.api
{
    [ApiController]
    [Route("api/ticket")]
    public class TicketController : Controller
    {

        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAll() {
            var users = await _ticketService.getAllTickets();

            return Ok(users);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<TicketDTO>> GetById(int id) {
            var ticket = await _ticketService.getTicketById();
            if (ticket == null) {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketDTO>> Create(CreateTicketDTO ticketInfo) {
            var ticket = await _ticketService.createTicket(ticketInfo);
            return Created(ticket);
        }

        [HttpPatch]
        public async Task<ActionResult<TicketDTO>> Update(UpdateTicketDTO ticketInfo) {
            var ticket = await _ticketService.updateTicket(ticketInfo);

            return Ok(ticket);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<DeleteTicketResultDTO>> Delete() {
            await _ticketService.deleteTicket();

            return Ok();
        }

    }
}
