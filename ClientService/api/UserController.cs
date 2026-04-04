using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using ClientService.dto;
using ClientService.service;
using ClientService.filter;

namespace ClientService.api
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<UserDto>> Get(string id) {
            var user = await _userService.GetUserByIdAsync(id);

            return Ok(user);
        } 

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto userInfo) {
            var user = await _userService.CreateUserAsync(userInfo);

            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<UserDto>> Update(UpdateUserDto userInfo) {
            var user = await _userService.UpdateUserAsync(userInfo);

            return Ok(user);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> Delete(string id) {
            await _userService.DeleteUserAsync(id);

            return Ok();
        }
    }
}
