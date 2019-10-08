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
        User GetUserByToken(string token);
        int insertPhoto(Photo photo);
        int GetPhotoId(string name);
        Photo GetPhoto(int id);
        Photo GetFirstPhotoFromUser(int userId);
        Photo GetLastPhotoFromUser(int userId);
    }
}
