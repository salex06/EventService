using AutoMapper;
using ClientService.dto;
using ClientService.entity;
using ClientService.exception;
using ClientService.repository;

namespace ClientService.service.impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null) 
                throw new NotFoundException($"Пользователь с id={id} не найден");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            string userName = createUserDto.Name;
            var user = await _userRepository.GetUserByNameAsync(userName);
            if (user != null)
                throw new BadRequestException($"Имя {userName} уже занято");

            var userToSave = _mapper.Map<User>(createUserDto);
            return _mapper.Map<UserDto>(await _userRepository.CreateUserAsync(userToSave));
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            string userId = updateUserDto.Id;
            var user = await _userRepository.GetUserAsync(userId);
            if (user == null)
                throw new NotFoundException($"Пользователь с id={userId} не найден");

            var newData = _mapper.Map<User>(updateUserDto);
            if (await _userRepository.GetUserByNameAsync(newData.Name) != null)
                throw new BadRequestException($"Имя {newData.Name} уже занято");

            return _mapper.Map<UserDto>(await _userRepository.UpdateUserAsync(newData));
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            if(!await _userRepository.DeleteUserAsync(id))
                throw new NotFoundException($"Пользователь с id={id} не найден");

            return true;
        }
    }
}
