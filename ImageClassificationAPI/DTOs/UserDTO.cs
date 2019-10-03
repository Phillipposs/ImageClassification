using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.DTOs
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }

    }
}
