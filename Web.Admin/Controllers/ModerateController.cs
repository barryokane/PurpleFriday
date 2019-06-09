using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Admin.Models;

namespace Web.Admin.Controllers
{
    [Authorize]
    public class ModerateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
