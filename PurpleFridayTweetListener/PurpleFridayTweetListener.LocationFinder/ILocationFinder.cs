using System;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.LocationFinder
{
    public interface ILocationFinder
    {
        Task<Coordinates> GetLocationFromStringAsync(string locationText);
    }
}
