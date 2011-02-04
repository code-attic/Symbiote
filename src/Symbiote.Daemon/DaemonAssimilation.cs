﻿// /* 
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
using System.Diagnostics;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon.BootStrap;
using Symbiote.Daemon.BootStrap.Config;
using Symbiote.Daemon.Host;
using Symbiote.Daemon.Installation;

namespace Symbiote.Daemon
{
    public static class DaemonAssimilation
    {
        public static IAssimilate Daemon( this IAssimilate assimilate, Action<DaemonConfigurator> config )
        {
            assimilate.Dependencies( x => x.Scan( s =>
                                                      {
                                                          s.TheCallingAssembly();
                                                          s.AssembliesFromApplicationBaseDirectory(
                                                              a =>
                                                              a.GetReferencedAssemblies().Any(
                                                                  r => r.Name.Contains( "Symbiote" ) ) );
                                                          s.AddAllTypesOf<IDaemon>();
                                                      } ) );

            var daemonConfiguration = new DaemonConfigurator();
            config( daemonConfiguration );
            var hostType = Process.GetCurrentProcess().Parent().ProcessName == "services"
                               ? typeof( DaemonHost )
                               : typeof( ConsoleHost );

            assimilate.Dependencies( x =>
                                         {
                                             x.For<DaemonConfiguration>().Use( daemonConfiguration.Configuration );
                                             x.For<IServiceCoordinator>().Use<ServiceCoordinator>();
                                             x.For<ICheckPermission>().Use<CredentialCheck>();
                                             x.For( typeof( ServiceController<> ) ).Use( typeof( ServiceController<> ) );
                                             x.For<IHost>().Use( hostType );
                                             x.For<IBootStrapper>().Use<BootStrapper>().AsSingleton();

                                             if (daemonConfiguration.Configuration.BootStrapConfiguration != null)
                                                x.For<BootStrapConfiguration>().Use(
                                                    daemonConfiguration.Configuration.BootStrapConfiguration);
                                         } );
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
            catch ( Exception e )
            {
                "No host configured. Wah. \r\n\t {0}"
                    .ToError<IDaemon>( e );
            }
        }
    }
}