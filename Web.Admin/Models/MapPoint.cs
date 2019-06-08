using System;
using Newtonsoft.Json;

namespace Web.Admin.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MapPoint
    {
        [JsonProperty("img")]
        public string Img { get; set; }
        [JsonProperty]
        public string Text { get; set; }
        [JsonProperty]
        public string TwitterHandle { get; set; }

        public MapPoint()
        {
        }
    }
}
