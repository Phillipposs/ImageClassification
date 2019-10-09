using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using ImageClassificationAPI.DTOs;
using ImageClassificationAPI.Services;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ImageClassificationAPI.Models;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Collections.Generic;

namespace ImageClassificationAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/")]
    public class ImageController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        public ImageController(IUserService userService, IHostingEnvironment environment, IPhotoService photoService)
        {
            _userService = userService;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _photoService = photoService;

        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _userService.CreateUser(user);
            return Ok();
        }

        [HttpGet("/getresult")]
        public async Task<IActionResult> GetResult([FromQuery(Name = "userName")]string userName)
        {

            string resultString = "";
            int userId = _userService.GetUserId(userName);
          User user = _userService.GetUser(userId);
            List<Photo> userPhotos = _photoService.GetUserPhotos(userId);
            foreach(Photo p in userPhotos)
            {
                string photoPath;
                string[] lines = { "", "" };
                photoPath = _environment.WebRootPath + "\\uploads\\reports\\" + p.Name + ".txt";
                
                if (System.IO.File.Exists(photoPath))
                {
                    lines = System.IO.File.ReadAllLines(photoPath);
                }
                resultString = resultString +"`"+ lines[0];
            }
            //string photoName = _photoService.GetLastPhotoFromUser(user.Id).Name;
            //int id = _photoService.GetPhotoId(photoName);
            //Photo photo = _photoService.GetPhoto(id);

            return Ok(resultString);

        }
        [HttpPost("/sendimage")]
        public async Task SendImage(IFormFile file, string userName)

        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            int id = _userService.GetUserId(userName);
            User user = _userService.GetUser(id);
            _photoService.InsertPhoto(new Photo
            {
                Name = file.FileName,
                UserId = user.Id
            });
            if (file.Length > 0)
            {
                string path = Path.Combine(uploads, file.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))

                {

                    await file.CopyToAsync(fileStream);

                   // SendToRabbitMQ(path+"`"+user.DeviceToken);

                }

            }

        }
        [HttpPost("/sendimages")]
        public async Task SendImages(List<IFormFile> files, string userName)

        {
            foreach (IFormFile file in files)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                int id = _userService.GetUserId(userName);
                User user = _userService.GetUser(id);
                _photoService.InsertPhoto(new Photo
                {
                    Name = file.FileName,
                    UserId = user.Id
                });
                if (file.Length > 0)
                {
                    string path = Path.Combine(uploads, file.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))

                    {

                        await file.CopyToAsync(fileStream);

                        // SendToRabbitMQ(path+"`"+user.DeviceToken);

                    }

                }
            }
           

        }
      


    }
}
