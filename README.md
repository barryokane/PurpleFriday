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

  ```
  "DataForwarder": {
    "BaseUri": "", 
    "SendTweetDataPath": ""
  }
  ```
 
## Listener
The listener communicates Twitter (either be listening to tweets, or replying) and also filters out tweets that don't successfully meet the criteria of having a location (or hashtag containing a location name). The requirement of having an image is optional and can be configured in these settings. 

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
  
  ## Logging
  The logging redirects the console logs to a file, configured in the Logging section of the appsettings. When logging to a file, the logs will create a new log file each time the application starts, with the file name prefixed with the time and date that the application was started. 
  
  The logging settings:
  * LogToFile - when true, the console output will be redirected to a log file in the `LogFolderPath`
  * LogFolderPath - an absolute file path to a directory that should be used for storing logs. Note: the application must have permissions to create, read and write files in the location.
  
  ```
    "Logging": {
    "LogToFile":  true,
    "LogFolderPath": "C:/Users/markm/Code/PurpleFriday/PurpleFridayTweetListener/PurpleFridayTweetListener/Logs/"
  }
  ```
