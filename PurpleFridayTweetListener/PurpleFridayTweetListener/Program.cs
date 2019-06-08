using System;
using System.Linq;
using Tweetinvi;

namespace PurpleFridayTweetListener
{
    class Program
    {
        private static string STREAM_FILTERS = "";
        static void Main(string[] args)
        {
            Auth.SetUserCredentials("cwDkQciFg4SMZ8xX4h5wGc4JL", "9XANRg9PkjqJtj3BI9B3UwQha2TfDgu2fdNPNAxm9cZ6TfDkuI", "975777483525165062-saRDxPN7t840ooILIHLRR5JLdjr8nXf", "jBcticse5V0r2oVrUutssIsLWWSRCgt5gdsok6xBvcApp");
            //var stream = Stream.CreateSampleStream();
            //stream.TweetReceived += (sender, tArgs) =>
            //{
            //    // Do what you want with the Tweet.
            //    Console.WriteLine(tArgs.Tweet);
            //};

            var stream = Stream.CreateFilteredStream();
            stream.AddTrack("#WorldOceansDay");
            var user = User.GetAuthenticatedUser();
            Console.WriteLine(user);

            stream.MatchingTweetReceived += (sender, targs) =>
            {
                if(targs.Tweet.Coordinates != null)
                {
                    Console.WriteLine($"{targs.Tweet.Coordinates.Latitude} {targs.Tweet.Coordinates.Longitude}");
                }
                else
                {
                    Console.WriteLine("No location data"); 
                }
                //Console.WriteLine($"{targs.Tweet.Text} {(targs.Tweet.Coordinates != null? $"{targs.Tweet.Coordinates.Latitude} {targs.Tweet.Coordinates.Longitude}" : "")}");
            };
            stream.StartStreamMatchingAllConditions();

            //stream.StartStream();
            stream.StreamStarted += (sender, streamArgs) => {
                Console.WriteLine("Stream started");
            };

            stream.StreamStopped += (sender, streamArgs) =>
            {
                var exceptionThatCausedTheStreamToStop = streamArgs.Exception;
                var twitterDisconnectMessage = streamArgs.DisconnectMessage;
                Console.WriteLine("Disconnected");
            };
            Console.ReadKey();
        }
    }
}
