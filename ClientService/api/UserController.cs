using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using ClientService.dto;
using ClientService.service;
using ClientService.filter;
using AutoMapper;

namespace ClientService.api
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<UserDto>> Get(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto userInfo)
        {
            var user = await _userService.CreateUserAsync(userInfo);

            return CreatedAtAction(nameof(Get), new { id = user.Id }, _mapper.Map<UserDto>(user));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult<UserDto>> Update(string id, UpdateUserDto userInfo)
        {
            var user = await _userService.UpdateUserAsync(id, userInfo);

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> Delete(string id)
        {
            await _userService.DeleteUserAsync(id);

            return Ok();
        }
    }
}
