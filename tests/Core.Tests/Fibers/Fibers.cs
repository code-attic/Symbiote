using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Core.Tests.Fibers
{
    public class Director
    {
        Action<object> Actor { get; set; }


    }

    public class Mailbox
    {
        public bool Processing { get; set; }
        public object ProcessLock { get; set; }
        public ConcurrentQueue<object> Messages { get; set; }

        public void Write( object message )
        {
            
        }

        public void Process( Action<object> action )
        {
            if( !Processing )
            {
                try
                {
                    Processing = true;
                    lock( ProcessLock )
                    {
                    
                    }
                }
                finally
                {
                    Processing = false;
                }
            }
            else
            {
                Thread.Sleep( 0 );
            }
        }
    }
}
