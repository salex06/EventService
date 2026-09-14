using AutoMapper;
using ClientService.dto;
using ClientService.entity;
using ClientService.exception;
using ClientService.service;
using Microsoft.Extensions.Options;

namespace ClientService.graphql
{
    public class Mutation
    {
        public async Task<User> CreateUser(CreateUserDto userDto, [Service] IUserService svc)
        {
            return await svc.CreateUserAsync(userDto);
        }

        public async Task<User> UpdateUser(string id, UpdateUserDto user, [Service] IUserService svc)
        {
            try
            {
                return await svc.UpdateUserAsync(id, user);
            }
            catch (ApiException ex)
            {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage(ex.Message)
                        .SetCode(ex.StatusCode.ToString())
                        .Build()
                );
            }
        }

        public async Task<bool> DeleteUser(string id, [Service] IUserService svc)
        {
            try
            {
                await svc.DeleteUserAsync(id);
                return true;
            }
            catch (ApiException ex)
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
