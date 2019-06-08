using System;
using System.Collections.Generic;
using System.Text;

namespace PurpleFridayTweetListener
{
    public class TweetListenerConfig
    {
        public string Filter { get; set; }
        public bool SendReply { get; set; }
        public string ReplyText { get; set; }

    }
}
