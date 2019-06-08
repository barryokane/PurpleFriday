using Geocoding;
using Geocoding.Google;
using Geocoding.Microsoft;
using NUnit.Framework;
using PurpleFridayTweetListener.LocationFinder;
using System.Threading.Tasks;

namespace Tests
{
    public class Tests
    {
        IGeocoder geocoder;
    
        [SetUp]
        public void Setup()
        {
            geocoder = new BingMapsGeocoder("Avmx12pASs7Py8SGg_nPgxHPF0eeUY3DzR7LKsPP9Su6toxQhnUudgJ5p-rOebFm");
        }

        [Test]
        public async Task Test1()
        {
            var locationFinder = new LocationFinder();

            var coords = await locationFinder.GetLocationFromStringAsync("Glasgow");
            
            Assert.NotNull(coords);
        }

        [Test]
        public async Task Test2()
        {

            var address = await geocoder.GeocodeAsync("United States");
            Assert.NotNull(address);
        }
    }
}