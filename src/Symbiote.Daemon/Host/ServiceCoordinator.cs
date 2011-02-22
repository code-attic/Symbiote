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
using System.Diagnostics;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon.BootStrap;

namespace Symbiote.Daemon.Host
{
    [DebuggerDisplay( "Hosting {HostedServiceCount} Services" )]
    public class ServiceCoordinator :
        IServiceCoordinator
    {
        private bool _disposed;
        protected IList<IServiceController> Services { get; set; }
        protected DaemonConfiguration Configuration { get; set; }
        public IBootStrapper BootStrap { get; set; }

        public void Start()
        {
            Services.ForEach( x => x.Start() );
            BootStrap.Start();
        }

        public void Stop()
        {
            Services.ForEach( x => x.Stop() );
        }

        public void Pause()
        {
        }

        public void Continue()
        {
        }

        public void StartService( string name )
        {
            Services.Where( x => x.Name.Equals( name ) ).First().Start();
        }

        public void StopService( string name )
        {
            Services.Where( x => x.Name == name ).First().Stop();
        }

        public void PauseService( string name )
        {
            Services.Where( x => x.Name == name ).First().Pause();
        }

        public void ContinueService( string name )
        {
            Services.Where( x => x.Name == name ).First().Continue();
        }

        public int HostedServiceCount
        {
            get { return Services.Count; }
        }

        public IList<ServiceInformation> GetServiceInfo()
        {
            return Services
                .ToList()
                .ConvertAll( serviceController => new ServiceInformation
                                                      {
                                                          Name = serviceController.Name,
                                                          State = serviceController.State,
                                                          Type = serviceController.ServiceType.Name
                                                      } );
        }

        public IServiceController GetService( string name )
        {
            return Services.Where( x => x.Name == name ).FirstOrDefault();
        }

        public void Dispose()
        {
            Dispose( true );
        }

        private void Dispose( bool disposing )
        {
            if ( _disposed )
                return;
            if ( disposing )
            {
                Services.ForEach( s => s.Dispose() );
                Services.Clear();
            }
            _disposed = true;
        }

        public void AddNewService( IServiceController controller )
        {
            Services.Add( controller );
        }

        public void RegisterServices()
        {
            Assimilate
                .Assimilation
                .DependencyAdapter
                .GetTypesRegisteredFor<IDaemon>()
                .Select( x => typeof( ServiceController<> ).MakeGenericType( x ) )
                .Select( CreateController )
                .ForEach( Services.Add );
        }

        protected IServiceController CreateController( Type type )
        {
            return Assimilate.GetInstanceOf( type ) as IServiceController;
        }

        public ServiceCoordinator( DaemonConfiguration configuration, IBootStrapper bootStrapper )
        {
            Services = new List<IServiceController>();
            Configuration = configuration;
            RegisterServices();
            BootStrap = bootStrapper;
        }
    }
}