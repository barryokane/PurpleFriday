using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Web.Admin.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Admin.Controllers
{

    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _settingsFilePath;
        public SettingsController(IHostingEnvironment host, IConfiguration configuration)
        {
            _settingsFilePath = host.ContentRootPath + @"/_datastore/settings.json";
            _configuration = configuration;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var settings = GetSettings(_settingsFilePath);

            var model = new SettingsPageModel {
                ApiKey = _configuration.GetValue<string>("IncomingMapAPIKey"),
                TweetResponse = settings
            };
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(TweetResponse tweetResponse)
        {
            SaveSettings(_settingsFilePath, tweetResponse);

            var model = new SettingsPageModel
            {
                ApiKey = _configuration.GetValue<string>("IncomingMapAPIKey"),
                TweetResponse = tweetResponse
            };
            return View(model);
        }

        TweetResponse GetSettings(string jsonFilePath)
        {
            if (!System.IO.File.Exists(jsonFilePath))
            {
                return new TweetResponse();
            }

            string json = System.IO.File.ReadAllText(jsonFilePath);
            TweetResponse settings = JsonConvert.DeserializeObject<TweetResponse>(json);
            return settings;
        }

        void SaveSettings(string jsonFilePath, TweetResponse settings)
        {
            // serialize JSON to a string and then write string to a file	
            System.IO.File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(settings));
        }
    }
}
