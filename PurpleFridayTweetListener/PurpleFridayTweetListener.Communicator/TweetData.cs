using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PurpleFridayTweetListener.Communicator
{
    public class TweetData
    {
        [JsonProperty(PropertyName ="text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "img")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "twitterHandle")]
        public string TwitterHandle { get; set; }

        [JsonProperty(PropertyName = "geo")]
        public double[] Coords { get; set; }

        public DateTime CreatedDate { get; set; }

        public string TweetId { get; set; }
    }
}
