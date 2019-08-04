using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Web.Admin.Data;
using Web.Admin.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Admin.Controllers
{

    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _settingsFolderPath;
        public SettingsController(IHostingEnvironment host, IConfiguration configuration)
        {
            _settingsFolderPath = host.ContentRootPath + @"/_datastore/";
            _configuration = configuration;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var settings = SettingsRepository.GetSettings(_settingsFolderPath);

            var model = new SettingsPageModel {
                ApiKey = _configuration.GetValue<string>("IncomingMapAPIKey"),
                TweetResponse = settings
            };
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(TweetResponse tweetResponse)
        {
            SettingsRepository.SaveSettings(_settingsFolderPath, tweetResponse);

            var model = new SettingsPageModel
            {
                ApiKey = _configuration.GetValue<string>("IncomingMapAPIKey"),
                TweetResponse = tweetResponse
            };
            ViewData["message"] = "Settings saved";
            return View(model);
        }

    }
}
