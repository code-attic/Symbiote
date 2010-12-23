using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeHealthMonitor :
        IObservable<NodeHealth>
    {
        TimeSpan MonitorFrequency { get; }
        NodeHealth LastStatus { get; }
    }

    public interface INodeHealthBroadcaster :
        IObserver<NodeHealth>
    {
        
    }
}
