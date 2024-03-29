﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.Services
{
    public class PhotoService : IPhotoService
    {
        private IUserRepository _userRepository;
        public PhotoService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Photo GetFirstPhotoFromUser(int userId)
        {
            return _userRepository.GetFirstPhotoFromUser(userId);
        }

        public Photo GetLastPhotoFromUser(int userId)
        {
            return _userRepository.GetLastPhotoFromUser(userId);
        }

        public Photo GetPhoto(int id)
        {
            return _userRepository.GetPhoto(id);
        }

        public int GetPhotoId(string name)
        {
            return _userRepository.GetPhotoId(name);
        }

        public List<Photo> GetUserPhotos(int userId)
        {
            return _userRepository.GetUserPhotos(userId);
        }

        public int InsertPhoto(Photo photo)
        {
            return _userRepository.insertPhoto(photo);
        }
    }
}
