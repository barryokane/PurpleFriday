using System;
using Web.Admin.Models;

namespace Web.Admin.Data.Models
{
    public class SqLiteMapPoint : MapPoint
    {
        public double Geo_x { get; set; }
        public double Geo_y { get; set; }

        public MapPoint ToMapPoint()
        {
            return new MapPoint()
            {
                Id = Id,
                TweetId = TweetId,
                Text = Text,
                TweetUrl = TweetUrl,
                Img = Img,
                TwitterHandle = TwitterHandle,
                LocationConfidence = LocationConfidence,
                CreatedDate = CreatedDate,
                Geo = new double[] { Geo_x, Geo_y },
                Area = Area,
                Hide = Hide
            };
        }

        public static MapPoint FromMapPoint(MapPoint mapPoint)
        {
            return new SqLiteMapPoint()
            {
                Id = mapPoint.Id,
                TweetId = mapPoint.TweetId,
                Text = mapPoint.Text,
                TweetUrl = mapPoint.TweetUrl,
                Img = mapPoint.Img,
                TwitterHandle = mapPoint.TwitterHandle,
                LocationConfidence = mapPoint.LocationConfidence,
                CreatedDate = mapPoint.CreatedDate,
                Geo_x = mapPoint.Geo[0],
                Geo_y = mapPoint.Geo[1],
                Area = mapPoint.Area,
                Hide = mapPoint.Hide
            };
        }
    }

}
