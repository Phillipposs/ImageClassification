using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageClassificationAPI.DTOs;
using ImageClassificationAPI.Entities;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.Services
{
    public class UserRepository : IUserRepository
    {
        private UserDbContext UserContext { get; }
        public UserRepository(UserDbContext leagueContext)
        {
            UserContext = leagueContext;
        }

        public List<UserDTO> GetUsers()
        {
            throw new NotImplementedException();
        }
        public User GetUser(int id)
        {
            return UserContext.Users
         .Where(u => u.Id == id).FirstOrDefault();
        }

        public int insertUser(string name, string password, string deviceToken)
        {
            var user = new User { Name = name, Password = password, DeviceToken = deviceToken};
            UserContext.Users.Add(user);
            if (UserContext.Users
            .Where(u => u.Name == name).FirstOrDefault() == null)
            {
                UserContext.SaveChanges();
                return user.Id;
            }
            else
                return GetUserId(name);
        }

        public int GetUserId(string name)
        {
            return UserContext.Users
            .Where(u => u.Name == name).SingleOrDefault().Id;
        }

        public int insertPhoto(Photo photo)
        {
            UserContext.Photos
           .Add(photo);
            if (UserContext.Photos
            .Where(p => p.Name == photo.Name).FirstOrDefault() == null)
            {
                UserContext.SaveChanges();
                return photo.Id;
            }
            else          
                return GetPhotoId(photo.Name);
        }

        public int GetPhotoId(string name)
        {
            return UserContext.Photos
            .Where(p => p.Name == name).SingleOrDefault().Id;
        }

        public Photo GetPhoto(int id)
        {
            return UserContext.Photos
         .Where(p => p.Id == id).FirstOrDefault();
        }

        public User GetUserByToken(string token)
        {
            return UserContext.Users
            .Where(u => u.DeviceToken == token).SingleOrDefault();
        }

        public Photo GetFirstPhotoFromUser(int userId)
        {
            return UserContext.Photos
            .Where(p => p.UserId == userId).FirstOrDefault();
        }

        public Photo GetLastPhotoFromUser(int userId)
        {
            return UserContext.Photos
  .Where(p => p.UserId == userId).LastOrDefault();
        }
    }
}
