using System;
using System.Collections.Generic;
using System.Text;

namespace PurpleFridayTweetListener.LocationFinder
{
    public class LocationFinderConfiguration
    {
        public string BingMapsKey { get; set; }
        private Dictionary<string, string> LocationOverrides { get; set; }
    }
}
