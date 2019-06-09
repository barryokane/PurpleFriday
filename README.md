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
