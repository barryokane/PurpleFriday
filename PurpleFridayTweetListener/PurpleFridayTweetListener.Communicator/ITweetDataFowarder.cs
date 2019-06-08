using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PurpleFridayTweetListener.Communicator
{
    public interface ITweetDataFowarder
    {
        Task ForwardData(TweetData data);
    }
}
