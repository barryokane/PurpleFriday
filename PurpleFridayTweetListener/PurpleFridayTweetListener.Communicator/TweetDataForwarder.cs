using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.Communicator
{
    public class TweetDataForwarder: ITweetDataFowarder
    {
        private readonly HttpClient _httpClient;
        private readonly TweetDataConfiguration _config;
        public TweetDataForwarder(TweetDataConfiguration config)
        {
            if (config.BaseUrl == null)
            {
                throw new ArgumentException("Base Url is required");
            }
            if (String.IsNullOrEmpty(config.TweetSendPath))
            {
                throw new ArgumentException("Tweet Send path is required");
            }
            _config = config;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = config.BaseUrl;
        }

        public async Task ForwardData(TweetData data)
        {
            var result = await _httpClient.PostAsync(_config.TweetSendPath, new JsonContent(data));
        }
    }
}
