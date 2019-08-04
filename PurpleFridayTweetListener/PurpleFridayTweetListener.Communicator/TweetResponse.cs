using System;
namespace PurpleFridayTweetListener.Communicator
{
    public class TweetResponse
    {
        public bool SendReplyTweet { get; set; }
        public string ReplyTweetText { get; set; }

        public TweetResponse()
        {
            this.SendReplyTweet = false;
        }
    }
}
