using Newtonsoft.Json;
using PurpleFridayTweetListener.Communicator;
using PurpleFridayTweetListener.LocationFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace PurpleFridayTweetListener
{
    public class TweetListener
    {
        private ILocationFinder _locationFinder;
        private TwitterAuthConfig _twitterAuthConfig;
        private TweetListenerConfig _listenerConfig;
        private IDataForwarderFactory _tweetForwarderFactory;

        public TweetListener(TwitterAuthConfig authConfig, TweetListenerConfig tweetListenerConfig, DataForwarderConfiguration tweetDataForwarderConfiguration, IDataForwarderFactory tweetForwarderFactory, ILocationFinder locationFinder)
        {
            _twitterAuthConfig = authConfig;
            _listenerConfig = tweetListenerConfig;
            _locationFinder = locationFinder;
            _tweetForwarderFactory = tweetForwarderFactory;

        }

        /// <summary>
        /// Starts a stream with the given credentaisl and a filter word
        /// </summary>
        /// <param name="config"></param>
        /// <param name="filter"></param>
        public void StartStream(string filter)
        {
            Auth.SetUserCredentials(_twitterAuthConfig.ConsumerKey, _twitterAuthConfig.ConsumerSecret, _twitterAuthConfig.UserAccessToken, _twitterAuthConfig.UserAccessSecret);

            var stream = Stream.CreateFilteredStream();
            stream.AddTrack(filter);

            var user = User.GetAuthenticatedUser();
            Console.WriteLine(user);

            stream.MatchingTweetReceived += async (sender, targs) =>
            {
                //don't include retweets or quotes
                if (targs.Tweet.QuotedTweet != null || targs.Tweet.RetweetedTweet != null)
                {
                    return;
                }

                //the tweet must have media
                if (targs.Tweet.Media == null || !targs.Tweet.Media.Any())
                {
                    return;
                }

                //the media must be photo or animated_gif (not video)
                if (targs.Tweet.Media.All(x => x.MediaType == "video"))
                {
                    return;
                }

                var forwardedTweetData = false;

                Console.WriteLine($"Tweet received: {targs.Tweet.Text}");
                
                if (targs.Tweet.Place != null)
                {
                    Console.WriteLine(targs.Json);
                    forwardedTweetData = await HandleTweetWithLocationData(targs.Tweet);
                }
                else
                {
                    forwardedTweetData = await HandleTweetWithoutLocationData(targs.Tweet, filter);
                }

                if (forwardedTweetData && _listenerConfig.SendReply)
                {
                    // We must add @screenName of the author of the tweet we want to reply to
                    var textToPublish = $"@{targs.Tweet.CreatedBy.ScreenName} {_listenerConfig.ReplyText}";
                    var tweet = Tweet.PublishTweetInReplyTo(textToPublish, targs.Tweet.Id);
                    Console.WriteLine($"Publish success: {tweet != null}");
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
                Console.WriteLine($"Disconnected {streamArgs.Exception.Message}");
            };

            stream.StartStreamMatchingAllConditions();
        }

        protected async Task<bool> HandleTweetWithLocationData(ITweet tweet)
        {
            Console.WriteLine("Tweet place bounding box:");
            foreach (var coord in tweet.Place.BoundingBox.Coordinates)
            {
                Console.WriteLine($"{coord.Latitude} - {coord.Longitude}");
            }

            var coordinateList = tweet.Place.BoundingBox.Coordinates.Select(x => new LocationFinder.Coordinates { Latitude = x.Latitude, Longitude = x.Longitude }).ToList();
            var centerPoint = _locationFinder.GetCentralGeoCoordinate(coordinateList);

            Console.WriteLine($"Center point: {centerPoint.Latitude} - {centerPoint.Longitude}");

            var tweetData = new TweetData
            {
                CreatedDate = tweet.CreatedAt,
                Image = tweet.Media.First(x => x.MediaType != "video").MediaURLHttps,
                Text = tweet.Text,
                TweetId = tweet.IdStr,
                Coords = new double[] { centerPoint.Latitude, centerPoint.Longitude },
                TwitterHandle = tweet.CreatedBy.ScreenName,
                LocationConfidence = LocationConfidence.EXACT.ToString(),
                TweetUrl = tweet.Url
            };

            return await ForwardTweetData(tweetData);
        }

        protected async Task<bool> HandleTweetWithoutLocationData(ITweet tweet, string filterHashtag)
        {
            //check all hashtags, one by one to see if they contain a location
            foreach (var hashtag in tweet.Hashtags.Where(x => $"#{x.Text}" != filterHashtag).ToList())
            {
                var locationResult = await _locationFinder.GetLocationFromStringAsync(hashtag.Text);
                if (locationResult == null)
                {
                    continue;
                }

                var tweetData = new TweetData
                {
                    CreatedDate = tweet.CreatedAt,
                    Image = tweet.Media.First(x => x.MediaType != "video").MediaURLHttps,
                    Text = tweet.Text,
                    TweetId = tweet.IdStr,
                    Coords = new double[] { locationResult.Coordinates.Latitude, locationResult.Coordinates.Longitude },
                    TwitterHandle = tweet.CreatedBy.ScreenName,
                    LocationConfidence = locationResult.Confidence.ToString(),
                    TweetUrl = tweet.Url
                };

                return await ForwardTweetData(tweetData);
                
            }

            return false;
        }

        private async Task<bool> ForwardTweetData(TweetData data)
        {
            var tweetForwarder = _tweetForwarderFactory.NewForwarder();

            try
            {
                await tweetForwarder.ForwardTweetData(data);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error forwarding data to client: {e.Message}");
                Console.WriteLine(JsonConvert.SerializeObject(data));
                return false;
            }

            return true;
        }
    }
}
