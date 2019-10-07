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
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Text;

namespace ImageClassificationAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/league")]
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

        [HttpPost("/sendpush")]
        public async Task SendPush(string fullPath, string fileName)

        {

            string name = fileName.Replace(".txt", "");
            int id = _photoService.GetPhotoId(name);
            Photo photo = _photoService.GetPhoto(id);
            User user = _userService.GetUser(photo.UserId);
            string userDeviceToken = user.DeviceToken;
            string content =System.IO.File.ReadLines(fullPath).First();
            SendPushNotificationFirebase(content, userDeviceToken);
        }

        [HttpPost("/league/sendimage")]
        public async Task Post(IFormFile file, string userName)

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
        public static void SendPushNotificationFirebase(string result, string deviceToken)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAoKD6ujk:APA91bGIlMTNyheEmDUgHedRTdunwBwO3gznSsBAZlHVozl7c47pK_JjHXlM_QJ7YDPDAh-C68LzZysEhtHFf8Hfy2gzoJVOytLvhR687vmT5_kEiwoR_AcvhoWrWYNQrkDW-vPmYshU"));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", "689895553593"));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = deviceToken, //"cY3S4j3pirY:APA91bG80jjo7x_Q-VO3V8hPxWJAiZoKsMH2AyIMVNluczcPDmvKOUUhteEMiidAtDKUgxOQGeP-cQA5LWj50tMjJbMJiRD9IV8iZo78ndmU4yIo-pwrZDwRraw-Cz6zpHLxDMNQzQHz",
                priority = "high",
                content_available = true,
                data = new
                {
                    result = result,
                    title = "Test",
                    badge = 1,
                    silent = true
                },
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                Console.WriteLine("Sent push notification !!!", result);
                                //result.Response = sResponseFromServer;
                            }
                    }
                }
            }
        }

        //[HttpGet("/league/getallleagues")]
        //public void GetAllLeagues()
        //{

        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(queue: "msgKey",
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        var msg = "dog.42.jpg";
        //        var body = Encoding.UTF8.GetBytes(msg);
        //        channel.BasicPublish(exchange: "",
        //                             routingKey: "msgKey",
        //                             basicProperties: null,
        //                             body: body);
        //    }


        //    Console.ReadLine();
        //}

        //private void SendToRabbitMQ(string argument)
        //{
        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    using (var connection = factory.CreateConnection())
        //    using (var channel = connection.CreateModel())
        //    {
        //        channel.QueueDeclare(queue: "msgKey",
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //        var msg = argument;
        //        var body = Encoding.UTF8.GetBytes(msg);
        //        channel.BasicPublish(exchange: "",
        //                             routingKey: "msgKey",
        //                             basicProperties: null,
        //                             body: body);
        //    }
        //}


    }
}
