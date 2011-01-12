using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl.Connection
{
    public class SingleConnectionProvider
        : IConnectionProvider
    {
        public IConnectionFactory Factory { get; set; }

        public IConnectionHandle Acquire()
        {
            return new SingleConnectionHandle( Factory.GetConnection() );
        }

        public SingleConnectionProvider( IConnectionFactory factory )
        {
            Factory = factory;
        }
    }
}
