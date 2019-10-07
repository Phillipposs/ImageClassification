﻿
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Classifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.callPython();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "msgKey",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                   // Program.SendPushNotificationFirebase("tesssst");

                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "msgKey",
                                     autoAck: true,
                                     consumer: consumer);
                Console.ReadLine();
            }

        }
        public static void callPython()
        {
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result

            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            //python interprater location
            start.FileName = @"C:\Users\Filip\AppData\Local\Programs\Python\Python37\python.exe";
            //argument with file name and input parameters
            start.Arguments = string.Format("{0} {1}", Path.Combine(projectDirectory, "Classifier.py"), @"C:\Users\Filip\source\repos\Phillipposs\ImageClassification\ImageClassificationAPI\wwwroot\uploads");
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            start.LoadUserProfile = true;
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    Console.WriteLine("From System Diagnostics");
                    Console.WriteLine(result);
                    // SendPushNotificationFirebase(result,messages[1]);
                }
            }
        }
        //public static void callPython()
        //{
        //    string workingDirectory = Environment.CurrentDirectory;
        //    // or: Directory.GetCurrentDirectory() gives the same result

        //    // This will get the current PROJECT directory
        //    string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        //    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
        //    //python interprater location
        //    start.FileName = @"C:\Users\Filip\AppData\Local\Programs\Python\Python37\python.exe";
        //    //argument with file name and input parameters
        //    start.Arguments = string.Format("{0} {1}", Path.Combine(projectDirectory, "Classifier.py"), "C:\\Users\\Filip\\source\\repos\\Phillipposs\\ImageClassification\\ImageClassificationAPI\\wwwrоot\\uploads");
        //    start.UseShellExecute = false;// Do not use OS shell
        //    start.CreateNoWindow = true; // We don't need new window
        //    start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
        //    start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
        //    start.LoadUserProfile = true;
        //    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
        //    {
        //        using (StreamReader reader = process.StandardOutput)
        //        {
        //            string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
        //            string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
        //            Console.WriteLine("From System Diagnostics");
        //            Console.WriteLine(result);
        //           // SendPushNotificationFirebase(result,messages[1]);
        //        }
        //    }
        //}
        
        public static void SendPushNotificationFirebase(string result,string deviceToken)
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
    }
    
}
