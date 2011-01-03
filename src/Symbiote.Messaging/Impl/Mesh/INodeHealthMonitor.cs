using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualBasic.Devices;
using Symbiote.Core.Utility;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeHealthMonitor :
        IObservable<NodeHealth>
    {
        NodeHealth LastStatus { get; }
        void Start();
        void Stop();
    }

    public class NodeHealthMonitor
        : INodeHealthMonitor
    {
        public ObserverCollection<NodeHealth> Observers { get; protected set; }
        public INodeConfiguration Configuration { get; protected set; }
        public NodeHealth LastStatus { get; protected set; }
        public Timer UpdateTimer { get; protected set; }

        public IDisposable Subscribe( IObserver<NodeHealth> observer )
        {
            return Observers.AddObserver( observer );
        }

        public void Start()
        {
            if(UpdateTimer == null)
            {
                UpdateTimer = new Timer(UpdateHealth, null, TimeSpan.Zero, Configuration.HealthMonitorFrequency);
            }
        }

        public void Stop()
        {
            UpdateTimer.Dispose();
            UpdateTimer = null;
        }

        public void UpdateHealth(object nullstate)
        {
            var computerInfo = new ComputerInfo();
            var availableRam = (decimal) computerInfo.AvailablePhysicalMemory / (decimal) computerInfo.TotalPhysicalMemory;
            Observers.OnEvent( new NodeHealth() { LoadScore = availableRam, NodeId = Configuration.IdentityProvider.Identity });
        }

        public NodeHealthMonitor( INodeConfiguration configuration, IEnumerable<INodeHealthBroadcaster> broadcasters )
        {
            Observers = new ObserverCollection<NodeHealth>();
            Configuration = configuration;
            broadcasters
                .ForEach( x => Subscribe(x) );
        }
    }

    public interface INodeHealthBroadcaster :
        IObserver<NodeHealth>
    {
        
    }

    public class NodeHealthBroadcaster
        : INodeHealthBroadcaster
    {
        public INodeConfiguration Configuration { get; set; }
        public IBus Bus { get; set; }

        public void OnNext( NodeHealth value )
        {
            Bus.Publish( Configuration.MeshChannel, value );
        }

        public void OnError( Exception error )
        {
            // do nothing
        }

        public void OnCompleted()
        {
            // do nothing
        }

        public NodeHealthBroadcaster( INodeConfiguration configuration, IBus bus )
        {
            Configuration = configuration;
            Bus = bus;
        }
    }
}
