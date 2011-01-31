// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
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
        public Timer UpdateTimer { get; protected set; }

        public NodeHealth LastStatus { get; protected set; }

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

        public void UpdateHealth( object sender, ElapsedEventArgs e )
        {
            var computerInfo = new ComputerInfo();
            var availableRam = computerInfo.AvailablePhysicalMemory/(decimal) computerInfo.TotalPhysicalMemory;
            Observers.OnEvent( new NodeHealth {LoadScore = 1, NodeId = Configuration.IdentityProvider.Identity} );
        }

        public NodeHealthMonitor( INodeConfiguration configuration, IEnumerable<INodeHealthBroadcaster> broadcasters )
        {
            Observers = new ObserverCollection<NodeHealth>();
            Configuration = configuration;
            UpdateTimer = new Timer( Configuration.HealthMonitorFrequency.TotalMilliseconds );
            UpdateTimer.Elapsed += UpdateHealth;
            broadcasters
                .ForEach( x => Subscribe( x ) );
        }
    }
}