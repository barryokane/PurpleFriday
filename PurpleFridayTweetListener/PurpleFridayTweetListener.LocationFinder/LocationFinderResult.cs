using System;
using System.Collections.Generic;
using System.Text;

namespace PurpleFridayTweetListener.LocationFinder
{
    public class LocationFinderResult
    {
        public Coordinates Coordinates { get; set; }
        public LocationConfidence Confidence { get; set; }
        public string AdminDistrict2 { get; set; }
    }
}
