using System;
using System.Collections.Generic;
using ImageClassificationAPI.DTOs;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.Services
{
    public interface IUserRepository
    {
  
        List<UserDTO> GetUsers();
        User GetUser(int id);
        int GetUserId(string name);
        int insertUser(string name,string password, string deviceToken);
    }
}
