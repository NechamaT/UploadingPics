using FileUploading.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileUploading.Data;

namespace FileUploading.Controllers
{


    public class HomeController : Controller
    {
        private string _connectionString =
            @"Data Source=.\sqlexpress;Initial Catalog=UploadImages;Integrated Security=true;";

        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {
            Guid guid = Guid.NewGuid();
            string actualFileName = $"{guid}-{imageFile.FileName}";
            string finalFileName = Path.Combine(_environment.WebRootPath, "uploads", actualFileName);
            using var fs = new FileStream(finalFileName, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            var db = new Imagesdb(_connectionString);
            var image = new Image
            {
                Name = actualFileName,
                Password = password,
                View = +1
            };
            db.Add(image);
            var vm = new UploadViewModel
            {
                Image = image
            };

            return View(vm);
        }

        public IActionResult ViewImage(int id, string password)
        {
            var db = new Imagesdb(_connectionString);
            var ids = HttpContext.Session.Get<List<int>>("ids");
            if (ids == null)
            {
                ids = new List<int>();
            }

            var sessionInProgress = ids.Any(i => i == id);
            Image img = db.GetImageById(id);
            bool correct = img.Password == password;
            bool msg = false;

            if (password != null)
            {
                msg = true;
            }

            bool showImage = sessionInProgress || correct;

            if (showImage)
            {
                db.UpdateViews(id);
            }

            if (!sessionInProgress || correct)
            {
                ids.Add(id);
                HttpContext.Session.Set("ids", ids);
            }

            var vm = new ViewImageViewModel()
            {
                Image = img,
                ShowImage =  showImage,
                ShowMessage = msg
            };

            return View(vm);

        }
    }
}

    public static class SessionExtensions
        {
            public static void Set<T>(this ISession session, string key, T value)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }

            public static T Get<T>(this ISession session, string key)
            {
                string value = session.GetString(key);
                return value == null ? default(T) :
                    JsonConvert.DeserializeObject<T>(value);
            }
        }
    
