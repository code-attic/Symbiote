using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualBasic.Devices;
using Symbiote.Core.Extensions;
using Symbiote.Core.Utility;

namespace Symbiote.Messaging.Impl.Mesh
{
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
}