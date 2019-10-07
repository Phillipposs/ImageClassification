using System.Collections.Generic;

namespace ImageClassificationAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
        public List<Photo> Photos { get; set; }
    }
}
