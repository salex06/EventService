using MongoDB.Bson;
using MongoDB.Driver;
using ClientService.entity;

namespace ClientService.repository.impl
{
    public class UserRepository : IUserRepository
    {

        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("users");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var filter = Builders<User>.Filter.Empty;

            return (await _users.FindAsync(filter)).ToList();
        }

        public async Task<User?> GetUserAsync(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(e => e.Id, id);

                return await _users.Find(filter).FirstOrDefaultAsync();
            } catch (FormatException) {
                return null;
            }
        }

        public async Task<User?> GetUserByNameAsync(string name) {
            var filter = Builders<User>.Filter.Eq(e => e.Name, name);

            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsUser(string id) {
            try
            {
                var filter = Builders<User>.Filter.Eq(e => e.Id, id);

                return (await _users.CountDocumentsAsync(filter)) != 0;
            }
            catch (FormatException) {
                return false;
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
           await _users.InsertOneAsync(user);

           return user;
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(t => t.Id, user.Id);

                await _users.ReplaceOneAsync(filter, user);

                return user;
            }
            catch (FormatException) {
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(t => t.Id, id);

                var deleteResult = await _users.DeleteOneAsync(filter);

                return deleteResult.DeletedCount != 0;
            }
            catch (FormatException) {
                return false;
            }
        }
    }
}
