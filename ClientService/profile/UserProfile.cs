using AutoMapper;
using ClientService.dto;
using ClientService.entity;

namespace ClientService.profile
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<CreateUserDto, User>();

            CreateMap<UpdateUserDto, User>();
        }
    }
}
