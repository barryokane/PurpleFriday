using System;
using System.Collections.Generic;
using System.Text;

namespace PurpleFridayTweetListener.Communicator
{
    public class DataForwarderConfiguration
    {
        public Uri BaseUrl { get; set; }
        public string SendTweetDataPath { get; set; }

        public string APIKeyHeaderName { get; set; }

        public string APIKey { get; set; }
    }
}
