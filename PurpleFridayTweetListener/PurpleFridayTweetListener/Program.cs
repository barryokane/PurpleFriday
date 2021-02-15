using Geocoding.Microsoft;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Nito.AsyncEx;
using PurpleFridayTweetListener.Communicator;
using PurpleFridayTweetListener.LocationFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using PurpleFridayTweetListener.Logger;
using System.Threading;

namespace PurpleFridayTweetListener
{   
    class Program
    {
        private static FileStream ostrm;
        private static StreamWriter writer;
        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }
        private static Task MainAsync(string[] args)
        {
            //https://blog.bitscry.com/2017/05/30/appsettings-json-in-net-core-console-app/
            //https://blogs.msdn.microsoft.com/fkaduk/2017/02/22/using-strongly-typed-configuration-in-net-core-console-app/
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot config = builder.Build();

            //PurpleFridayTweetListener.

            var streamConfig = new TwitterAuthConfig();
            config.Bind("TwitterCredentials", streamConfig);

            var dataForwarderConfig = new DataForwarderConfiguration();
            config.Bind("DataForwarder", dataForwarderConfig);
            dataForwarderConfig.BaseUrl = new Uri(config["DataForwarder:BaseUri"]);

            if (bool.Parse(config["Logging:LogToFile"]))
            {
                // Log to console and file.
                Logging.SetupLogging(config["Logging:LogFolderPath"],config["Logging:LogToSingleFile"]);          
            }
            else
            {
                // Log to console only.
                Logging.SetupLogging();
            }

            Logging.Information("Starting PurpleFridayTweetListener");

            Logging.Information("Putting in 10 second wait so we don't spam Twitter in the event of a restart");
            Thread.Sleep(10000);
            Logging.Information("Sleep finished");

            var tweetListenerConfig = new TweetListenerConfig();
            config.Bind("Listener", tweetListenerConfig);

            var locationFinderConfig = new LocationFinderConfiguration();
            config.Bind("LocationFinder", locationFinderConfig);

            var locationFinder = new LocationFinder.LocationFinder(locationFinderConfig, new BingMapsGeocoder(locationFinderConfig.BingMapsKey));

            var tweetListener = new TweetListener(streamConfig, tweetListenerConfig, new DataForwarderFactory(dataForwarderConfig), locationFinder);

            tweetListener.StartStream(args.Any()? args[0]: tweetListenerConfig.Filter);

            Console.Read();

            return null;
        }
    }
}
