using System;
using System.Collections.Generic;
using System.Text;

namespace PurpleFridayTweetListener.Communicator
{
    public interface IDataForwarderFactory
    {
        IDataFowarder NewForwarder();
    }
}
