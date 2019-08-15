using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.Communicator
{
    public class DataForwarder: IDataFowarder
    {
        private readonly HttpClient _httpClient;
        private readonly DataForwarderConfiguration _config;
        public DataForwarder(DataForwarderConfiguration config)
        {
            if (config.BaseUrl == null)
            {
                throw new ArgumentException("Base Url is required");
            }
            if (String.IsNullOrEmpty(config.SendTweetDataPath))
            {
                throw new ArgumentException("Send path is required");
            }
            _config = config;
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add(_config.APIKeyHeaderName, _config.APIKey);
            _httpClient.BaseAddress = config.BaseUrl;
        }

        /// <summary>
        /// Post Tweet to API
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Text string to use in response tweet (null or empty for no response)</returns>
        public async Task<string> ForwardTweetData(TweetData data)
        {
            var response = await _httpClient.PostAsync(_config.SendTweetDataPath, new JsonContent(data));
            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                try {
                    var tweetResponse = JsonConvert.DeserializeObject<TweetResponse>(jsonString);
                    return (tweetResponse.SendReplyTweet) ? tweetResponse.ReplyTweetText : null;
                } catch (Exception e) {
                    throw new Exception("Invalid repsonse from Map API: "+jsonString, e);
                }
            }

            return null;
        }
    }
}
