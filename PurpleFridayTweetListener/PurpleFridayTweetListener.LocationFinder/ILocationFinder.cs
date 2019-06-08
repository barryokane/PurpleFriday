using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.LocationFinder
{
    public interface ILocationFinder
    {
        Task<LocationFinderResult> GetLocationFromStringAsync(string locationText);
        Coordinates GetCentralGeoCoordinate(IList<Coordinates> geoCoordinates);
    }
}
