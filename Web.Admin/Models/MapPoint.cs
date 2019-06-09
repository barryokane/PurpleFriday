using System;
using Newtonsoft.Json;

namespace Web.Admin.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MapPoint
    {
        bool? _hide = false;
        [JsonProperty]
        public bool? Hide { 
            get {
                return _hide;
            }  
            set
            {
                _hide = (value.HasValue) ? value : false;
            }
        }

        [JsonProperty]
        public string TweetId { get; set; }
        [JsonProperty("img")]
        public string Img { get; set; }
        [JsonProperty]
        public string Text { get; set; }
        [JsonProperty]
        public DateTime CreatedDate { get; set; }
        [JsonProperty]
        public string CreatedDateDisplay
        {
            get
            {
                return $"{CreatedDate.ToShortDateString()} {CreatedDate.ToShortTimeString()}";
            }
        }
        [JsonProperty]
        public string TwitterHandle { get; set; }
        [JsonProperty]
        public string TweetUrl { get; set; }
        [JsonProperty]
        public string LocationConfidence { get; set; }
        [JsonProperty]
        public double[] Geo { get; set; }

        public MapPoint()
        {
        }
    }
}
