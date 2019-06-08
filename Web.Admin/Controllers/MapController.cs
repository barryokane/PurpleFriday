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
        public JsonResult Get(bool? getall)
        {
            List<MapPoint> mapData = (getall.HasValue && getall.Value) ? GetData(dataFilePath) : GetDataNotHidden(dataFilePath);
            return Json(mapData);
        }
        [HttpGet("{id}")]
        public JsonResult Get(string id)
        {
            MapPoint point = GetDataSingle(id);
            return Json(point);
        }

        /// <summary>
        /// Test hardcoded data
        /// </summary>
        /// <returns>Hardcoded list of map data</returns>
        [HttpGet("test")]
        public JsonResult Test()
        {
            List<MapPoint> mapData = GetDataNotHidden(host.ContentRootPath + @"/_data/MapData_Test.json");
            return Json(mapData);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]MapPoint value)
        {
            //todo: validation?
            AddNewData(value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody]MapPoint point)
        {
            UpdateData(id, point);
        }

        // DELETE api/values/5
        /*[HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/

        List<MapPoint> GetDataNotHidden(string jsonFilePath)
        {
            List<MapPoint> mapData = GetData(jsonFilePath);
            mapData = (from m in mapData
                       where m.Hide.HasValue || !m.Hide.Value
                       select m).ToList();
            return mapData;
        }
        List<MapPoint> GetData(string jsonFilePath)
        {
            string json = System.IO.File.ReadAllText(jsonFilePath);
            List<MapPoint> mapData = JsonConvert.DeserializeObject<List<MapPoint>>(json);
            return mapData;
        }

        MapPoint GetDataSingle(string id)
        {
            List<MapPoint> mapData = GetData(dataFilePath);
            MapPoint point = mapData.FirstOrDefault(x => x.TweetId == id);
            return point;
        }

        void AddNewData(MapPoint point)
        {
            List<MapPoint> mapData = GetData(dataFilePath);
            //todo: check for duplicates?
            mapData.Add(point);

            // serialize JSON to a string and then write string to a file
            System.IO.File.WriteAllText(dataFilePath, JsonConvert.SerializeObject(mapData));
        }

        void UpdateData(string id, MapPoint point)
        {
            List<MapPoint> mapData = GetData(dataFilePath);

            //remove the one we are replacing
            List<MapPoint> newData = mapData.Where(m => m.TweetId != id).ToList();

            //add the new one
            newData.Add(point);

            // serialize JSON to a string and then write string to a file
            System.IO.File.WriteAllText(dataFilePath, JsonConvert.SerializeObject(newData));
        }
    }
}
