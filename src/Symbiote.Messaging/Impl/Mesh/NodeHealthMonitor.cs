using System;
using System.Collections.Generic;
using System.Timers;
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
            UpdateTimer.Start();
        }

        public void Stop()
        {
            UpdateTimer.Stop();
        }

        public void UpdateHealth(object sender, ElapsedEventArgs e)
        {
            var computerInfo = new ComputerInfo();
            var availableRam = (decimal) computerInfo.AvailablePhysicalMemory / (decimal) computerInfo.TotalPhysicalMemory;
            Observers.OnEvent( new NodeHealth() { LoadScore = 1, NodeId = Configuration.IdentityProvider.Identity });
        }

        public NodeHealthMonitor( INodeConfiguration configuration, IEnumerable<INodeHealthBroadcaster> broadcasters )
        {
            Observers = new ObserverCollection<NodeHealth>();
            Configuration = configuration;
            UpdateTimer = new Timer(Configuration.HealthMonitorFrequency.TotalMilliseconds);
            UpdateTimer.Elapsed += UpdateHealth;
            broadcasters
                .ForEach( x => Subscribe(x) );
        }
    }
}