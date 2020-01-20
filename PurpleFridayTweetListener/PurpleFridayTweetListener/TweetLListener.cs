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
using PurpleFridayTweetListener.Logger;

namespace PurpleFridayTweetListener
{
    public class TweetListener
    {
        private ILocationFinder _locationFinder;
        private TwitterAuthConfig _twitterAuthConfig;
        private TweetListenerConfig _listenerConfig;
        private IDataForwarderFactory _tweetForwarderFactory;

        public TweetListener(TwitterAuthConfig authConfig, TweetListenerConfig tweetListenerConfig, IDataForwarderFactory tweetForwarderFactory, ILocationFinder locationFinder)
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

                //ignore the tweet if it must have media
                if (_listenerConfig.ImageRequired && (targs.Tweet.Media == null || !targs.Tweet.Media.Any()))
                {
                    return;
                }

                //the media must be photo or animated_gif (not video)
                if (targs.Tweet.Media.All(x => x.MediaType == "video"))
                {
                    return;
                }

                string tweetResponseText;

                Logging.Information($"Tweet received: {targs.Tweet.Text}");
                
                if (targs.Tweet.Place != null)
                {
                    Logging.Debug(targs.Json);
                    tweetResponseText = await HandleTweetWithLocationData(targs.Tweet);
                }
                else
                {
                    tweetResponseText = await HandleTweetWithoutLocationData(targs.Tweet, filter);
                }

                if (!string.IsNullOrEmpty(tweetResponseText))
                {
                    // We must add @screenName of the author of the tweet we want to reply to
                    var textToPublish = $"@{targs.Tweet.CreatedBy.ScreenName} {tweetResponseText}";
                    var tweet = Tweet.PublishTweetInReplyTo(textToPublish, targs.Tweet.Id);
                    Logging.Debug($"Publish success: {tweet != null}");
                }
            };

            stream.StreamStarted += (sender, streamArgs) =>
            {
                Logging.Information("Stream started");
            };

            stream.StreamStopped += (sender, streamArgs) =>
            {
                var exceptionThatCausedTheStreamToStop = streamArgs.Exception;
                var twitterDisconnectMessage = streamArgs.DisconnectMessage;
                Logging.Fatal($"Disconnected {streamArgs.Exception.Message}");
            };

            stream.StartStreamMatchingAllConditions();
        }

        protected async Task<string> HandleTweetWithLocationData(ITweet tweet)
        {
            Logging.Debug("Tweet place bounding box:");
            foreach (var coord in tweet.Place.BoundingBox.Coordinates)
            {
                Logging.Debug($"{coord.Latitude} - {coord.Longitude}");
            }

            var coordinateList = tweet.Place.BoundingBox.Coordinates.Select(x => new LocationFinder.Coordinates { Latitude = x.Latitude, Longitude = x.Longitude }).ToList();
            var centerPoint = _locationFinder.GetCentralGeoCoordinate(coordinateList);

            Logging.Debug($"Center point: {centerPoint.Latitude} - {centerPoint.Longitude}");

            var tweetData = new TweetData
            {
                CreatedDate = tweet.CreatedAt,
                Image = tweet.Media.First(x => x.MediaType != "video").MediaURLHttps,
                Text = tweet.Text,
                TweetId = tweet.IdStr,
                Coords = new double[] { centerPoint.Latitude, centerPoint.Longitude },
                Area = "TODO", //todo: get area for this location!
                TwitterHandle = tweet.CreatedBy.ScreenName,
                LocationConfidence = LocationConfidence.EXACT.ToString(),
                TweetUrl = tweet.Url
            };

            return await ForwardTweetData(tweetData);
        }

        protected async Task<string> HandleTweetWithoutLocationData(ITweet tweet, string filterHashtag)
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
                    Area = locationResult.AdminDistrict2,
                    TwitterHandle = tweet.CreatedBy.ScreenName,
                    LocationConfidence = locationResult.Confidence.ToString(),
                    TweetUrl = tweet.Url
                };

                return await ForwardTweetData(tweetData);
                
            }

            return null;
        }

        private async Task<string> ForwardTweetData(TweetData data)
        {
            var tweetForwarder = _tweetForwarderFactory.NewForwarder();
            string responseText;

            try
            {
                responseText = await tweetForwarder.ForwardTweetData(data);

            }
            catch (Exception e)
            {
                Logging.Error($"Error forwarding data to client: {e.Message}");
                Logging.Debug(JsonConvert.SerializeObject(data));
                return null;
            }

            return responseText;
        }
    }
}
