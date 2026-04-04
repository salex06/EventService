using ClientService.entity;

namespace ClientService.repository
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetAllUsersAsync();

        public Task<User?> GetUserAsync(string id);

        public Task<User?> GetUserByNameAsync(string name);

        public Task<bool> ExistsUser(string id);

        public Task<User> CreateUserAsync(User user);

        public Task<User?> UpdateUserAsync(User user);

        public Task<bool> DeleteUserAsync(string id);
    }
}
