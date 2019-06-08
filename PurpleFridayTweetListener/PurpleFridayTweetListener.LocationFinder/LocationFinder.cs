using Geocoding;
using Geocoding.Google;
using Geocoding.Microsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.LocationFinder
{
    public class LocationFinder: ILocationFinder
    {
        private IGeocoder _geoCoder;
        public LocationFinder()
        {
            _geoCoder = new BingMapsGeocoder("Avmx12pASs7Py8SGg_nPgxHPF0eeUY3DzR7LKsPP9Su6toxQhnUudgJ5p-");
        }

        public async Task<Coordinates> GetLocationFromStringAsync(string locationText)
        {
            try
            {
                IEnumerable<Address> addresses = await _geoCoder.GeocodeAsync(locationText);
                //Console.WriteLine("Formatted: " + addresses.First().FormattedAddress); //Formatted: 1600 Pennsylvania Ave SE, Washington, DC 20003, USA
                //Console.WriteLine("Coordinates: " + addresses.First().Coordinates.Latitude + ", " + addresses.First().Coordinates.Longitude); //Coordinates: 38.8791981, -76.9818437
                //var coords = addresses.First().Coordinates;
                //return new Coordinates
                //{
                //    Latitude = coords.Latitude.ToString(),
                //    Longitude = coords.Longitude.ToString()
                //};
                return null;
            }
            catch(Exception e)
            {
                return null;
            }   

        }

    }
}
