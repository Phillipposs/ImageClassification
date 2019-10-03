using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using ImageClassificationAPI.DTOs;
using ImageClassificationAPI.Services;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using ImageClassificationAPI.Models;

namespace ImageClassificationAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/league")]
    public class ImageController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IUserService _userService;
        static FileSystemWatcher _watcher;

        /// <summary>
        /// Init.
        /// </summary>
        static void Init()
        {
            Console.WriteLine("INIT");
            string directory = "D:\\Master\\sk\\ImageClassificationServer\\ImageClassificationAPI\\wwwroot\\uploads\\reports";
            ImageController._watcher = new FileSystemWatcher(directory);
            ImageController._watcher.Changed +=
                new FileSystemEventHandler(ImageController._watcher_Changed);
            ImageController._watcher.EnableRaisingEvents = true;
            ImageController._watcher.IncludeSubdirectories = true;
        }

        /// <summary>
        /// Handler.
        /// </summary>
        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("CHANGED, NAME: " + e.Name);
            Console.WriteLine("CHANGED, FULLPATH: " + e.FullPath);
            // Can change program state (set invalid state) in this method.
            // ... Better to use insensitive compares for file names.
        }
        public ImageController(IUserService userService, IHostingEnvironment environment)
        {
            _userService = userService;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Init();

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



        [HttpPost("/league/sendimage")]
        public async Task Post(IFormFile file, string userName)

        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            int id = _userService.GetUserId(userName);
            User user = _userService.GetUser(id);
            if (file.Length > 0)
            {
                string path = Path.Combine(uploads, file.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))

                {

                    await file.CopyToAsync(fileStream);

                    SendToRabbitMQ(path+"`"+user.DeviceToken);

                }

            }

        }

        [HttpGet("/league/getallleagues")]
        public void GetAllLeagues()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "msgKey",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var msg = "dog.42.jpg";
                var body = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish(exchange: "",
                                     routingKey: "msgKey",
                                     basicProperties: null,
                                     body: body);
            }


            Console.ReadLine();
        }

        private void SendToRabbitMQ(string argument)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "msgKey",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var msg = argument;
                var body = Encoding.UTF8.GetBytes(msg);
                channel.BasicPublish(exchange: "",
                                     routingKey: "msgKey",
                                     basicProperties: null,
                                     body: body);
            }
        }


    }
}
