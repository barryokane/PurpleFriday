using Geocoding;
using Geocoding.Google;
using Geocoding.Microsoft;
using Newtonsoft.Json;
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
        private readonly LocationFinderConfiguration _configuration;

        public LocationFinder(LocationFinderConfiguration configuration, IGeocoder geocoder)
        {
            _configuration = configuration;
            _geoCoder = geocoder;
        }

        public async Task<LocationFinderResult> GetLocationFromStringAsync(string locationText)
        {
            try
            {
                IEnumerable<BingAddress> addresses = await _geoCoder.GeocodeAsync(locationText) as IEnumerable<BingAddress>;

                if(addresses == null || !addresses.Any())
                {
                    return null;
                }

                var matchedLocation = addresses.Where(x => x.Type == EntityType.PopulatedPlace 
                                            && x.Confidence <= ConfidenceLevel.Medium
                                            && x.CountryRegion == "United Kingdom"
                                            && x.AdminDistrict == "Scotland"
                                            ).FirstOrDefault();
                if(matchedLocation == null)
                {
                    return null;
                }

                Console.WriteLine("Formatted: " + addresses.First().FormattedAddress);
                Console.WriteLine("Coordinates: " + addresses.First().Coordinates.Latitude + ", " + addresses.First().Coordinates.Longitude);
                return new LocationFinderResult
                {
                    AdminDistrict2 = (matchedLocation.AdminDistrict2 == "Western Isles" ) ?
                                    "Na h-Eileanan Siar" : matchedLocation.AdminDistrict2,
                    Coordinates = new Coordinates
                    {
                        Latitude = matchedLocation.Coordinates.Latitude,
                        Longitude = matchedLocation.Coordinates.Longitude
                    },
                    Confidence = matchedLocation.Confidence == ConfidenceLevel.High ? LocationConfidence.HIGH : LocationConfidence.MEDIUM
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught in GetLocationFromStringAsync method, message: {e.Message}");
                Console.WriteLine(JsonConvert.SerializeObject(e));
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
