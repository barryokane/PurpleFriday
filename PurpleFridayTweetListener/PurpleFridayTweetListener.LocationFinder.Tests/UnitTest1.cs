using Geocoding;
using Geocoding.Google;
using Geocoding.Microsoft;
using NUnit.Framework;
using PurpleFridayTweetListener.LocationFinder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.LocationFinder
{
    public class Tests
    {
        IGeocoder geocoder;
        private string MAPS_KEY = "your map api key here";
        private static string NOT_NULL_FILENAME = "not_null_locations.txt";

        [SetUp]
        public void Setup()
        {
            geocoder = new BingMapsGeocoder(MAPS_KEY);
        }

        [Test]
        public async Task BaseGeoTest()
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync("Edinburgh");

            Assert.NotNull(coords);
            Assert.AreEqual(coords.AdminDistrict2, "City of Edinburgh");
        }
        [Test]
        public async Task Alva()
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync("Alva");

            Assert.NotNull(coords);
            Assert.AreEqual(coords.AdminDistrict2, "Clackmannanshire");
        }
        [Test]
        public async Task TestScotlandHack()
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync("Edinburgh,Scotland");

            Assert.NotNull(coords);
        }
        [Test]
        public async Task TestSkye()
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync("Skye");

            Assert.NotNull(coords);
        }
        [Test]
        public async Task TestForScotlandOnly()
        {
            Dictionary<string, string> testLocations = new Dictionary<string, string>();
            testLocations["Banff"] = "Aberdeenshire";
            testLocations["Inverkeithing"] = "Fife";
            testLocations["Perth"] = "Perth and Kinross";
            testLocations["johnogroats"] = "Highland";


            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            foreach (var location in testLocations)
            {
                var coords = await locationFinder.GetLocationFromStringAsync(location.Key);

                Assert.NotNull(coords);
                Assert.AreEqual(coords.AdminDistrict2, location.Value);

                System.Threading.Thread.Sleep(1000);//don't overload bing
            }
        }
        [Test]
        public async Task TestWesternIsles()
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync("Stornoway");

            Assert.NotNull(coords);
            Assert.AreEqual(coords.AdminDistrict2, "Na h-Eileanan Siar");
        }
        [TestCaseSource("NotNullTestCases")]
        public async Task NotNullLocations(string location)
        {
            var locationFinder = new LocationFinder(new LocationFinderConfiguration { BingMapsKey = MAPS_KEY }, geocoder);

            var coords = await locationFinder.GetLocationFromStringAsync(location);
            Assert.NotNull(coords);
            System.Threading.Thread.Sleep(100);//don't overload bing
        }
        public static IEnumerable<TestCaseData> NotNullTestCases()
        {
            var testCases = new List<TestCaseData>();

            string[] lines = System.IO.File.ReadAllLines(@"not_null_locations.txt");

            foreach (string line in lines)
            {
                testCases.Add(new TestCaseData(line));
            }

            if  (testCases != null)
            {
                foreach (TestCaseData testCaseData in testCases)
                {
                     yield return testCaseData;
                }
            }
         }
        [Test]
        public async Task Test2()
        {

            var address = await geocoder.GeocodeAsync("United States");
            Assert.NotNull(address);
        }
    }
}