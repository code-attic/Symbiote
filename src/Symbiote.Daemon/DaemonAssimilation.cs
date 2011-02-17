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
using System.Threading;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon.Installation;

namespace Symbiote.Daemon
{
    public static class DaemonAssimilation
    {
        public static IAssimilate Daemon( this IAssimilate assimilate, Action<DaemonConfigurator> config )
        {
            var daemonConfiguration = new DaemonConfigurator();
            config(daemonConfiguration);
            assimilate
                .Dependencies( container => container.For<DaemonConfiguration>().Use( daemonConfiguration.Configuration ) );
            return assimilate;
        }

        public static void RunDaemon( this IAssimilate assimilate )
        {
            try
            {
                "Waking the Daemon..."
                    .ToInfo<IDaemon>();
                var factory = Assimilate.GetInstanceOf<CommandProvider>();
                var command = factory.GetServiceCommand();
                command.Execute();
            }
            catch ( ThreadAbortException threadAbortException )
            {
                "The Daemon's thread has been aborted."
                    .ToWarn<IDaemon>();
                Thread.ResetAbort(); // Stops propagation here. The Daemon is dead.
            }
            catch ( Exception e )
            {
                "No host configured. \r\n\t {0}"
                    .ToError<IDaemon>( e );
            }
        }
    }
}