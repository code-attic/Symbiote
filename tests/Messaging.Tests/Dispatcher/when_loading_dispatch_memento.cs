using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Dispatch;

namespace Messaging.Tests.Dispatcher 
{
    public class when_loading_dispatch_memento 
    {
        public static IMemento<DispatchManager> memento { get; set; }

        private Establish context = () => 
        { 
            Assimilate.Initialize();
        };

        private Because of = () => 
        { 
            memento = Assimilate.GetInstanceOf<IMemento<DispatchManager>>();
        };

        private It should_not_return_null_instance = () => memento.ShouldNotBeNull();
    }
}
