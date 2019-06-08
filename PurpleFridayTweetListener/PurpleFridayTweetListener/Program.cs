using Newtonsoft.Json;
using PurpleFridayTweetListener.Communicator;
using PurpleFridayTweetListener.LocationFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;

namespace PurpleFridayTweetListener
{
    class Program
    {
        private static string STREAM_FILTERS = "#TroopingTheColour";
        private static ILocationFinder _locationFinder = new LocationFinder.LocationFinder();
        static async Task Main(string[] args)
        {
            Auth.SetUserCredentials("cwDkQciFg4SMZ8xX4h5wGc4JL", "9XANRg9PkjqJtj3BI9B3UwQha2TfDgu2fdNPNAxm9cZ6TfDkuI", "975777483525165062-saRDxPN7t840ooILIHLRR5JLdjr8nXf", "jBcticse5V0r2oVrUutssIsLWWSRCgt5gdsok6xBvcApp");

            var stream = Stream.CreateFilteredStream();
            stream.AddTrack(STREAM_FILTERS);
            var user = User.GetAuthenticatedUser();
            Console.WriteLine(user);

            stream.MatchingTweetReceived += async (sender, targs) =>
            {
                //TODO: Ignore retweets and ones with no image attached 
                if (targs.Tweet.QuotedTweet != null || targs.Tweet.RetweetedTweet != null)
                {
                    return;
                }

                if(targs.Tweet.Media == null)
                {
                    return;
                }

                Console.WriteLine($"{targs.Tweet.Text} {(targs.Tweet.Coordinates != null ? $"{targs.Tweet.Coordinates.Latitude} {targs.Tweet.Coordinates.Longitude}" : "")}");
                if (targs.Tweet.Place != null)
                {
                    Console.WriteLine(targs.Json);
                    Console.WriteLine("Place bounding box");
                    foreach (var coord in targs.Tweet.Place.BoundingBox.Coordinates)
                    {
                        Console.WriteLine($"{coord.Latitude} - {coord.Longitude}");
                    }
                    Console.WriteLine();
                    var coordinateList = targs.Tweet.Place.BoundingBox.Coordinates.Select(x => new Coordinates { Latitude = x.Latitude, Longitude = x.Longitude }).ToList();
                    var centerPoint = _locationFinder.GetCentralGeoCoordinate(coordinateList);
                    Console.WriteLine($"{centerPoint.Latitude} - {centerPoint.Longitude}");

                    var tweetForwarder = new TweetDataForwarder(new TweetDataConfiguration
                    {
                        BaseUrl = new Uri("https://127.0.0.1:5001"),
                        TweetSendPath = "/api/map"
                    });

                    var tweetData = new TweetData
                    {
                        CreatedDate = targs.Tweet.CreatedAt,
                        Image = targs.Tweet.Media.First().URL,
                        Text = targs.Tweet.Text,
                        TweetId = targs.Tweet.IdStr,
                        Coords = new double[] { centerPoint.Latitude, centerPoint.Longitude },
                        TwitterHandle = targs.Tweet.CreatedBy.ScreenName
                    };

                    try
                    {
                        await Task.Run(() => tweetForwarder.ForwardData(tweetData));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error forwarding data to client:");
                        Console.WriteLine(JsonConvert.SerializeObject(tweetData));
                    }
                }
                else
                {
                    //TODO: Lookup hashtags locations
                }


            };

            stream.StreamStarted += (sender, streamArgs) =>
            {
                Console.WriteLine("Stream started");
            };

            stream.StreamStopped += (sender, streamArgs) =>
            {
                var exceptionThatCausedTheStreamToStop = streamArgs.Exception;
                var twitterDisconnectMessage = streamArgs.DisconnectMessage;
                Console.WriteLine("Disconnected");
            };

            stream.StartStreamMatchingAllConditions();

            Console.ReadKey();
            return;

        }
    }
}
