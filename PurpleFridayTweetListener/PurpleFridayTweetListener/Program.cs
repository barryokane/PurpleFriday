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

namespace PurpleFridayTweetListener
{
    class Program
    {
        private static FileStream ostrm;
        private static StreamWriter writer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        ///     -file = Send output to log file (log file created with current timestamp and ends when application closes
        /// </param>
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

            var streamConfig = new TwitterAuthConfig();
            config.Bind("TwitterCredentials", streamConfig);

            var dataForwarderConfig = new DataForwarderConfiguration();
            config.Bind("DataForwarder", dataForwarderConfig);
            dataForwarderConfig.BaseUrl = new Uri(config["DataForwarder:BaseUri"]);

            var tweetListenerConfig = new TweetListenerConfig();
            config.Bind("Listener", tweetListenerConfig);

            if (bool.Parse(config["Logging:LogToFile"]))
            {
                SetUpWriteToFile(config["Logging:LogFolderPath"]);
            }


            var tweetListener = new TweetListener(streamConfig, tweetListenerConfig, dataForwarderConfig, new DataForwarderFactory(dataForwarderConfig), new LocationFinder.LocationFinder());

            tweetListener.StartStream(args.Any()? args[0]: tweetListenerConfig.Filter);

            Console.ReadKey();

            return null;
        }

        private static void SetUpWriteToFile(string fileName)
        {

            try
            {
                ostrm = new FileStream(Path.Combine(Environment.CurrentDirectory, $"{fileName}{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt"), FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
                writer.AutoFlush = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open {fileName} for writing");
                Console.WriteLine(e.Message);
                return;
            }

            Console.SetOut(writer);
        }


    }
}
