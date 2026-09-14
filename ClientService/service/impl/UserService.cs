using AutoMapper;
using ClientService.dto;
using ClientService.entity;
using ClientService.exception;
using ClientService.kafka.producer;
using ClientService.repository;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientService.service.impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IKafkaMessageProducer _producer;
        private readonly ProducerSettings _producerSettings;
        public UserService(
            IUserRepository userRepository, 
            IKafkaMessageProducer producer, 
            IOptions<ProducerSettings> producerSettings) {
            _userRepository = userRepository;
            _producer = producer;
            _producerSettings = producerSettings.Value;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null) 
                throw new NotFoundException($"Пользователь с id={id} не найден");

            return user;
        }

        public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
        {
            string userName = createUserDto.Name;
            var user = await _userRepository.GetUserByNameAsync(userName);
            if (user != null)
                throw new BadRequestException($"Имя {userName} уже занято");

            var userToSave = new User { 
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                RegisteredObjects = 0
            };
            return await _userRepository.CreateUserAsync(userToSave);
        }

        public async Task<User> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
                throw new NotFoundException($"Пользователь с id={id} не найден");

            if (await _userRepository.GetUserByNameAsync(updateUserDto.Name) != null)
                throw new BadRequestException($"Имя {updateUserDto.Name} уже занято");

            if (updateUserDto.Name != null) user.Name = updateUserDto.Name;
            if (updateUserDto.Email != null) user.Email = updateUserDto.Email;

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            if(!await _userRepository.DeleteUserAsync(id))
                throw new NotFoundException($"Пользователь с id={id} не найден");

            return true;
        }

        public async Task ConfirmObject(RegObjectDto regObjectDto) {
            var userId = regObjectDto.ConfirmatorId;
            var user = await _userRepository.GetUserAsync(userId);
            if (user != null) {
                user.RegisteredObjects++;
                await _userRepository.UpdateUserAsync(user);

                RegObjectResponseDto res = new RegObjectResponseDto
                {
                    ConfirmatorId = userId,
                    ObjId = regObjectDto.ObjectId,
                    ObjType = regObjectDto.Type,
                    ConfirmDateTime = DateTime.UtcNow
                };

                await _producer.SendMessageAsync(_producerSettings.TopicName, JsonSerializer.Serialize(res));
            }
        }
    }
}
