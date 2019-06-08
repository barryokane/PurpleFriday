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
            _httpClient.BaseAddress = config.BaseUrl;
        }

        public async Task ForwardTweetData(TweetData data)
        {
            var result = await _httpClient.PostAsync(_config.SendTweetDataPath, new JsonContent(data));
        }
    }
}
