using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.LocationFinder
{
    public interface ILocationFinder
    {
        Task<Coordinates> GetLocationFromStringAsync(string locationText);
        Coordinates GetCentralGeoCoordinate(IList<Coordinates> geoCoordinates);
    }
}
