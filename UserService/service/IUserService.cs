using ClientService.dto;

namespace ClientService.service
{
    public interface IUserService
    {
        public Task<IEnumerable<UserDto>> GetAllUsersAsync();

        public Task<UserDto> GetUserByIdAsync(string id);

        public Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

        public Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto);

        public Task<bool> DeleteUserAsync(string id);
    }
}
