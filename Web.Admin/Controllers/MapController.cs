using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Runtime.Serialization;
using Web.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Web.Admin.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string API_KEY;
        private readonly IMapPointRepository db;

        public MapController(IHostingEnvironment host, IConfiguration configuration) {

            _configuration = configuration;

            API_KEY = _configuration.GetValue<string>("IncomingMapAPIKey");

            var dataFolderPath = host.ContentRootPath + @"/_datastore/";
            db = new SqLiteMapPointRepository(dataFolderPath);
        }

        // GET: api/values
        [HttpGet]
        public JsonResult Get(bool? getall)
        {
            List<MapPoint> mapData = db.GetAll(getall.HasValue && getall.Value);
            return Json(mapData);
        }
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            MapPoint point = db.Get(id);
            return Json(point);
        }


        // POST api/values
        /// <summary>
        /// Save new Tweet to be shown on map
        /// </summary>
        /// <param name="value"></param>
        /// <returns>JSON that includes if reply tweet should be sent and if so what the text should be</returns>
        [HttpPost]
        public JsonResult Post([FromBody]MapPoint value)
        {
            if (!AuthHeaderIsValid())
            {
                return Json(new object[] { "Access denied" });
            }

            //todo: validation?
            db.AddNew(value);

            return Json(new TweetResponse());
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody]MapPoint point)
        {
            if (!AuthHeaderIsValid())
            {
                return;
            }

            db.Update(point);
        }

        // DELETE api/values/5
        /*[HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/


        private bool AuthHeaderIsValid()
        {
            var authHeader = (string)Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(API_KEY) || authHeader == null || authHeader != API_KEY)
            {
                return false;
            }

            return true;
        }
    }
}
