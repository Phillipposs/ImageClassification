using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageClassificationAPI.DTOs;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.Services
{
    public interface IUserService
    {
        User GetUser(int id);
        int CreateUser(UserDTO user);
        int GetUserId(string name);
        User GetUserByToken(string token);
    }
}
