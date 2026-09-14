using ClientService.dto;
using ClientService.entity;

namespace ClientService.service
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetAllUsersAsync();

        public Task<User> GetUserByIdAsync(string id);

        public Task<User> CreateUserAsync(CreateUserDto createUserDto);

        public Task<User> UpdateUserAsync(string id, UpdateUserDto updateUserDto);

        public Task<bool> DeleteUserAsync(string id);
        public Task ConfirmObject(RegObjectDto regObjectDto);
    }
}
