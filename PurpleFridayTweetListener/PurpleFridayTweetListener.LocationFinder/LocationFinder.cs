using Geocoding;
using Geocoding.Google;
using Geocoding.Microsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.LocationFinder
{
    public class LocationFinder : ILocationFinder
    {
        private IGeocoder _geoCoder;
        private static readonly string BING_KEY = "Avmx12pASs7Py8SGg_nPgxHPF0eeUY3DzR7LKsPP9Su6toxQhnUudgJ5p-rOebFm";

        public LocationFinder()
        {
            _geoCoder = new BingMapsGeocoder("Avmx12pASs7Py8SGg_nPgxHPF0eeUY3DzR7LKsPP9Su6toxQhnUudgJ5p-rOebFm");
        }

        public async Task<Coordinates> GetLocationFromStringAsync(string locationText)
        {
            try
            {
                IEnumerable<Address> addresses = await _geoCoder.GeocodeAsync(locationText);

                if(addresses == null || !addresses.Any())
                {
                    return null;
                }

                Console.WriteLine("Formatted: " + addresses.First().FormattedAddress);
                Console.WriteLine("Coordinates: " + addresses.First().Coordinates.Latitude + ", " + addresses.First().Coordinates.Longitude); 
                var coords = addresses.First().Coordinates;
                return new Coordinates
                {
                    Latitude = coords.Latitude,
                    Longitude = coords.Longitude
                };
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public Coordinates GetCentralGeoCoordinate(IList<Coordinates> geoCoordinates)
        {
            if (geoCoordinates.Count == 1)
            {
                return geoCoordinates.Single();
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in geoCoordinates)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = geoCoordinates.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new Coordinates
            {
                Latitude = centralLatitude * 180 / Math.PI,
                Longitude = centralLongitude * 180 / Math.PI
            };
        }

    }
}
