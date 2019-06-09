using Geocoding;
using Geocoding.Google;
using Geocoding.Microsoft;
using NUnit.Framework;
using PurpleFridayTweetListener.LocationFinder;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.LocationFinder
{
    public class Tests
    {
        IGeocoder geocoder;
        private string MAPS_KEY = "Avmx12pASs7Py8SGg_nPgxHPF0eeUY3DzR7LKsPP9Su6toxQhnUudgJ5p-rOebFm";

        [SetUp]
        public void Setup()
        {
            geocoder = new BingMapsGeocoder(MAPS_KEY);
        }

        [Test]
        public async Task Test1()
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync("Alva");

            Assert.NotNull(coords);
            Assert.AreEqual(coords.AdminDistrict2, "Clackmannanshire");
        }
        [Test]
        public async Task TestWesternIsles()
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync("Stornoway");

            Assert.NotNull(coords);
            Assert.AreEqual(coords.AdminDistrict2, "Na h-Eileanan Siar");
        }

        [Test]
        public async Task Test2()
        {

            var address = await geocoder.GeocodeAsync("United States");
            Assert.NotNull(address);
        }
    }
}