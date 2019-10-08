using ImageClassificationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageClassificationAPI.Services
{
    public interface IPhotoService
    {
         int GetPhotoId(string name);
         int InsertPhoto(Photo photo);
         Photo GetPhoto(int id);
         Photo GetFirstPhotoFromUser(int userId);
         Photo GetLastPhotoFromUser(int userId);
    }
}
