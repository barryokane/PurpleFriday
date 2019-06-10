using System;
using System.Collections.Generic;
using System.Text;

namespace PurpleFridayTweetListener.Communicator
{
    public class DataForwarderFactory : IDataForwarderFactory
    {
        private readonly DataForwarderConfiguration _config;
        public DataForwarderFactory(DataForwarderConfiguration config)
        {
            _config = config;
        }

        public IDataFowarder NewForwarder()
        {
            return new DataForwarder(_config);
        }
    }
}
