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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : Controller
    {
        private readonly IHostingEnvironment host;
        private readonly string dataFilePath;

        public MapController(IHostingEnvironment host) {
            this.host = host;
            dataFilePath = host.ContentRootPath + @"/_data/MapData.json";
        }

        // GET: api/values
        [HttpGet]
        public JsonResult Get()
        {
            List<MapPoint> mapData = GetData(dataFilePath);
            return Json(mapData);
        }
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            List<MapPoint> mapData = GetData(dataFilePath);
            MapPoint point = mapData.FirstOrDefault(x=>x.TweetId==id);
            return Json(point);
        }
        [HttpGet("test")]
        public JsonResult Test()
        {
            List<MapPoint> mapData = GetData(host.ContentRootPath + @"/_data/MapData_Test.json");
            return Json(mapData);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]MapPoint value)
        {
            List<MapPoint> mapData = GetData(dataFilePath);
            //todo: check for duplicates?
            mapData.Add(value);

            // serialize JSON to a string and then write string to a file
            System.IO.File.WriteAllText(dataFilePath, JsonConvert.SerializeObject(mapData));
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        
        List<MapPoint> GetData(string jsonFilePath)
        {
            string json = System.IO.File.ReadAllText(jsonFilePath);
            List<MapPoint> mapData = JsonConvert.DeserializeObject<List<MapPoint>>(json);
            return mapData;
        }
    }
}
