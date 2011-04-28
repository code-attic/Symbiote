using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Concurrency;

namespace Core.Tests.Concurrency.EventLoop
{
    public class with_event_loop
    {
        protected static Symbiote.Core.Concurrency.EventLoop Loop;

        private Establish context = () => { 
            Loop = new Symbiote.Core.Concurrency.EventLoop();
        };
    }
}
