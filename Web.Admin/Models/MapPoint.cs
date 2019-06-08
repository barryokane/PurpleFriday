using System;
using Newtonsoft.Json;

namespace Web.Admin.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MapPoint
    {
        [JsonProperty]
        public string TweetId { get; set; }
        [JsonProperty("img")]
        public string Img { get; set; }
        [JsonProperty]
        public string Text { get; set; }
        [JsonProperty]
        public DateTime CreatedDate { get; set; }
        [JsonProperty]
        public string TwitterHandle { get; set; }
        [JsonProperty]
        public double[] Geo { get; set; }

        public MapPoint()
        {
        }
    }
}
