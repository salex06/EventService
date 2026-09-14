using ClientService.entity;
using ClientService.exception;
using ClientService.service;

namespace ClientService.graphql
{
    public class Query
    {
        public async Task<IEnumerable<User>> GetAllUsers([Service] IUserService svc) {
            return await svc.GetAllUsersAsync();
        }

        public async Task<User> GetUserById(string id, [Service] IUserService svc) {
            try
            {
                return await svc.GetUserByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage(ex.Message)
                        .SetCode(ex.StatusCode.ToString())
                        .Build()
                );
            }
        }
    }
}
