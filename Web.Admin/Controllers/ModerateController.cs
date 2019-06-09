using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Web.Admin.Models;

namespace Web.Admin.Controllers
{
    [Authorize]
    public class ModerateController : Controller
    {
        private readonly IConfiguration _configuration;
        public ModerateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View(new ModeratePageModel { ApiKey = _configuration.GetValue<string>("IncomingMapAPIKey") });
        }
    }
}
