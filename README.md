# PurpleFriday
#impact48 hack with LGBT Youth, Scotland


# Tweet Listener

The tweet listener app listens to a stream of tweets from Twitter, filtered to a specific hashtag, user or keyword. When the app receives a tweet, it will forward it on to a URL with specific information contained in the tweet. 

The application is split into three parts:
1. **TweetListener** - this is the part of the app that listens to tweets and communicates with the other two parts
2. **DataForwarder** - this forwards on data to another application via an API call
3. **LocationFinder** - this is used to find the location of tweets that don't have a place. It searches for a location based on all of the hashtags in the tweet. 

## Pre-requisites

Before starting, you will need to set up a couple of services that are required for the application to work correctly.
1. Twitter API keys
  * Set up a developer account on Twitter to get your api auth tokens. The URL is https://developer.twitter.com/
  * Click create App
  * Note keys and secrets - these will be required next
2. Bing maps api key
  * Go to https://www.bingmapsportal.com/Application# and follow the steps to get a maps API key

## Configuration
The application is configured via the `appsettings.json`:

### TwitterCredentials
These are the keys that you got from the Twitter developer account earlier, they are:
 * ConsumerKey
 * ConsumerSecret
 * UserAccessToken
 * UserAccessSecret

### DataForwarder
The data forwarder is the application that is expecting the tweet data, complete with location. There are two configuration settings for the DataForwarder:
* BaseUri - the base URL for the DataForwarder application e.g. https://localhost.local/
* SendTweetDataPath - the API path expecting the tweet info
* APIKeyHeaderName - this is the name of the header that the web app expects for the api key
* APIKey - this is the API key for the website that will be sent with each request

  ```
  "DataForwarder": {
    "BaseUri": "", 
    "SendTweetDataPath": "",
    "APIKeyHeaderName": "e",
    "APIKey": ""
  }
  ```
 
### Listener
The settings for the Listener are:

 * Filter - this can be a # hashtag or a keyword and is used to tell Twitter that you're only interested in specific tweets
 * ReplyText - This is a string that will be sent in a reply to a successful (matches criteria) tweet. The reply tweet will come from the same account as the one used for the API credentials.
 * SendReply - a boolean used to determine if the application should reply to tweets
 * ImageRequired - if set to true, the application will only forward on tweets that have images (or animated gifs) attached

```
  "Listener": {
    "Filter": "#impact48",
    "ReplyText": "Thanks for tweeting us!",
    "SendReply": true,
    "ImageRequired": true
  }
  ```
  
  ### Location Finder
  The location finder uses Microsoft's Bing maps and needs an API key for it to work. The instructions for how to gain a key are in the pre-requisites section above. 
  
  ```
    "LocationFinder": {
    "BingMapsKey": ""
  }
  ```
  
  ### Logging
  The logging redirects the console logs to a file, configured in the Logging section of the appsettings. When logging to a file, the logs will create a new log file each time the application starts, with the file name prefixed with the time and date that the application was started. 
  
  The logging settings:
  * LogToFile - when true, the console output will be redirected to a log file in the `LogFolderPath`
  * LogToSingleFile - when true, will only write to file in LogFolderPath; when false, will roll the file each day and add a timestamp in the name.
  * LogFolderPath - a file path to a directory that should be used for storing logs. Note: the application must have permissions to create, read and write files in the location.
  
  ```
    "Logging": {
      "LogToFile":  true,
      "LogToSingleFile": true,
      "LogFolderPath": "C:/Users/markm/Code/PurpleFriday/PurpleFridayTweetListener/PurpleFridayTweetListener/Logs/"
  }
  ```
  
  ## Running the application
  The application uses `.net core 2.2` and can be run on Windows, Mac and Linux using the .net core command line. If you navigate to the `PurpleFridayTweetListener` folder in the command line and run:
  `dotnet run`
  you should see the log messages:
  
  ```
  {Twitter API name}
  Stream started
  ```
which will indicate that the app has successfully started and the Twitter stream is listening.

  ## Running the application in Docker (Linux/Mac)

  To create local Docker images, run the following command from the top of the repository:

```
./create_docker_images.sh
```

  To run the application, run the following script. It accepts 4 parameters:
```
./start_purplefriday.sh    # Parameter 1: Home directory for persistent files e.g. _datastore, logs etc.
                           #     Works in order of precednce:
                           #        1. PFHOME environment variable (eg. an exported variable outside script)
                           #        2. Parameter 1 inputted to this script
                           #        3. $HOME for the user running the script.
                           # --
                           # Parameter 2: Should we set up the directory structure (Y/[N]). 
                           # --
                           # Parameter 3: Running mode. dev (default) or prod 
                           # --
                           # Parameter 4: Start Monitoring? Default is (N)o.
                           # --
```

Examples:

```
./start_purplefriday.sh 
                            - If directory structure is set up,
                              The PFHOME environment variable is exported elsewhere
                              The running mode is dev.
                              Does not run monitoring.

./start_purplefriday.sh "${HOME}/PF" "Y" "prod" "Y"
                            - To set up the directory structure against ${HOME}/PF.
                              And then run PurpleFriday in prod mode with HTTPS. Also runs monitoring.

```
To stop the application, run:

```
./stop_purplefriday.sh
```

# How it works

## Listener 
The listener communicates Twitter (either be listening to tweets, or replying) and also filters out tweets that don't successfully meet the criteria of having a location (or hashtag containing a location name). The requirement of having an image is optional and can be configured in these settings. 

  ## Location Finder
  The location finder uses Microsoft's Bing maps to try and identify a location from a hashtag in a Tweet, if the Tweet doesn't contain its own location data. The application iterates through all of the hashtags in a tweet and searches each one using the maps API until it finds a `MEDIUM` or `HIGH` match. If no match is found, the tweet is ignored and not forwarded on. 
  
  ## Data Forwarder
  Once the application has a tweet that matches all of the criteria, including a location, it will then forward the data using the DataForwarder settings in the configuration. 
  The format of the data forwarded is: 
  
  ```
  {
   "tweetId":"", //the tweet id 
   "img":"", //the url of the tweet's first image
    "text":"", //the tweet's text
    "createdDate":"2019-06-09T10:47:52+01:00", //the datetime of the tweet (yyyy-MM-ddTHH:mm:ss ISO)            
    "twitterHandle":"", //the user's screen name who tweeted
    "tweetUrl":"", // the url of the original tweet
    "locationConfidence":"", //EXACT (location from tweet), HIGH (bing had high confidence in location), or MEDIUM (bing had medium confidence)
    "geo":[] //[latitude, longitude] - as a double[]
  }
  ```
  

# Web Admin
The web admin project handles the loading of the map and has the API for the incoming data from the tweet listener. It also contains an authorised moderataor section to hide tweets from the map. 

## Configuration
The web adming contains an `appsettings.json` file in the root folder with the following settings: 

* IncomingMapAPIKey - this is that the API calls into the site from the tweet listener will be checked against
* Authentication Username - this is the username for the moderator section
* Authentication Password - this is the password for the moderator section

```
  "IncomingMapAPIKey": "",
  "Authentication": {
    "Username": "",
    "Password":  ""
  }
```

## HTTPS Support (Linux/Mac)
This application has HTTPS support for Docker containers via Traefik, with the setup defined in `traefik\traefik.toml`.

## Monitoring (Linux/Mac)
This application also comes with Monitoring built-in, carried out by:
* Grafana - For visualisation and alerting.
* Prometheus - For collector aggregation.
* Grok_Exporter - For log file pattern collection.
* cAdvisor - For container metric collection.

These all run within Docker containers, which you can choose to run as part of `start_purplefriday.sh`.

When all the containers are running, you can load Grafana at [http://localhost:3000/]. Default username and password is admin:admin.

After you have added Prometheus as a data source (<http://prometheus:9090>, I would recommend importing the following dashboard: <https://grafana.com/grafana/dashboards/193> to begin monitoring your application.

### Prometheus.yml

The following configuration file advises Prometheus to pull in the metics from cAdvisor and Grok_Exporter every 5 seconds.

```
scrape_configs:
- job_name: cadvisor
  scrape_interval: 5s
  static_configs:
  - targets:
    - cadvisor:8080
- job_name: grok_exporter
  scrape_interval: 5s
  static_configs:
  - targets:
    - grok_exporter:9144
```

This configuration file must be visible to the Prometheus container and is set up as follows:

```
    command:
        - --config.file=/etc/prometheus/prometheus.yml
    volumes:
        - ./prometheus.yml:/etc/prometheus/prometheus.yml:ro
```

### Grok_Exporter.yml

Grok_Exporter allows you to pull out metrics by pattern matching log files - here is an extract of how it is used in PurpleFriday:

```
metrics:
    - type: counter
      name: tweets_received_total
      help: Total number of tweets received
      match: '%{TIMESTAMP_ISO8601}%{GREEDYDATA}Tweet received'
    - type: counter
      name: fatal_errors
      help: The different types of Fatal Error being observed
      match: '%{TIMESTAMP_ISO8601}%{GREEDYDATA}\[FTL\] %{GREEDYDATA:err_type}'
      labels:
          err_type: '{{.err_type}}'
```

This is collating metrics based on the tweets received, and the number of fatal errors (and type) that it finds in the log file.

This file must also be mapped in the `docker-compose.yml`:

```
    volumes:
        - ./logstash-patterns-core:/logstash-patterns-core
        - ./grok_exporter.yml:/etc/grok_exporter/config.yml
        - ${PFHOME}/logs:/app/logs
```


## Deployment

### MS Azure
- Web Admin project can be dployed straight to Azure using Visual Studio
- The Tweet listener can be deployed as a WebJob on Azure. (TODO: update instructions here, but basically publish as an exe and upload as a zip file via Azure portal).

### Deploy to Linux 
* Requires Docker
* Manual deploy: Run deploy.sh
  * Option 1 to build & create Docker images
  * Option 2 to upload and deploy the Docker images (you will need remote server connection details)
  * Option 3 to restart remote Docker containers (useful if TweetListener stops working!)

//todo: add some more deployment help notes here...
