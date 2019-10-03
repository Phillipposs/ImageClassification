using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageClassificationAPI.DTOs;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public int CreateUser(UserDTO user)
        {
            var userId = _userRepository.insertUser(user.Name, user.Password, user.DeviceToken);
            return userId;
        }

        public User GetUser(int id)
        {
           return _userRepository.GetUser(id);
        }

        public int GetUserId(string name)
        {
            return _userRepository.GetUserId(name);
        }
    }
}
