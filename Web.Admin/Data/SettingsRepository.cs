using System;
using System.IO;
using Newtonsoft.Json;
using Web.Admin.Models;

namespace Web.Admin.Data
{
    public static class SettingsRepository
    {
        private const string jsonFileName = "settings.json";

        public static TweetResponse GetSettings(string jsonFilePath)
        {
            jsonFilePath = Path.Combine(jsonFilePath, jsonFileName);

            if (!File.Exists(jsonFilePath))
            {
                return new TweetResponse();
            }

            string json = System.IO.File.ReadAllText(jsonFilePath);
            TweetResponse settings = JsonConvert.DeserializeObject<TweetResponse>(json);
            return settings;
        }

        public static void SaveSettings(string jsonFilePath, TweetResponse settings)
        {
            jsonFilePath = Path.Combine(jsonFilePath, jsonFileName);

            // serialize JSON to a string and then write string to a file	
            System.IO.File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(settings));
        }
    }
}
