﻿using ImageClassificationAPI.Models;
using ImageClassificationAPI.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace ImageClassificationAPI
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static FileSystemWatcher _watcher;
        static IUserService _userService;
        static IHostingEnvironment _environment;
        static IPhotoService _photoService;

        /// <summary>
        /// Init.
        /// </summary>
        public Program(IUserService userService, IHostingEnvironment environment, IPhotoService photoService)
        {
            _userService = userService;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _photoService = photoService;

        }
        static void Init()
        {
            Console.WriteLine("INIT");
            string directory = @"C:\Users\Filip\source\repos\Phillipposs\ImageClassification\ImageClassificationAPI\wwwroot\uploads\reports";
            Program._watcher = new FileSystemWatcher(directory);
            Program._watcher.Changed +=
                new FileSystemEventHandler(Program._watcher_Changed);
            Program._watcher.EnableRaisingEvents = true;
            Program._watcher.IncludeSubdirectories = true;
        }

        /// <summary>
        /// Handler.
        /// </summary>
        static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
             CreateRequestAsync(client, e.FullPath,e.Name);
            // Can change program state (set invalid state) in this method.
            // ... Better to use insensitive compares for file names.
        }


        public static void Main(string[] args)
        {


            Task.Run(() => Init());

            CreateWebHostBuilder(args).Build().Run();



        }
        //public static void callPython()
        //{
        //    string workingDirectory = Environment.CurrentDirectory;
        //    // or: Directory.GetCurrentDirectory() gives the same result

        //    // This will get the current PROJECT directory
        //    //string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        //    System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
        //    //python interprater location
        //    start.FileName = @"C:\Users\Filip\AppData\Local\Programs\Python\Python37\python.exe";
        //    //argument with file name and input parameters
        //    start.Arguments = string.Format("{0} {1}", Path.Combine(workingDirectory, "Classifier.py"), @"C:\Users\Filip\source\repos\Phillipposs\ImageClassification\ImageClassificationAPI\wwwroot\uploads");
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
        //            // SendPushNotificationFirebase(result,messages[1]);
        //        }
        //    }
        //}


       
        static async void CreateRequestAsync(HttpClient client,string fullPath,string fileName)
        {
            var values = new Dictionary<string, string>
            {
                { "fullPath",fullPath},
                { "fileName",fileName}
            };

            var content = new FormUrlEncodedContent(values);

             client.PostAsync("https://192.168.0.101:45455/sendpush", content);

        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
