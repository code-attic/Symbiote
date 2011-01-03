using System;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeHealthMonitor :
        IObservable<NodeHealth>
    {
        NodeHealth LastStatus { get; }
        void Start();
        void Stop();
    }
}
