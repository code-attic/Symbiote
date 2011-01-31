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
using System.Diagnostics;
using Symbiote.Core;

namespace Symbiote.Daemon.Host
{
    //TODO: Missing a LOT of implementation!?
    [DebuggerDisplay( "Service {Name} is {State}" )]
    public class ServiceController<TService> :
        IServiceController
        where TService : class, IDaemon
    {
        protected bool Initialized { get; set; }
        protected TService ServiceInstance { get; set; }
        protected bool Disposed { get; set; }
        public Action<TService> PauseAction { get; set; }
        public Action<TService> ContinueAction { get; set; }

        public string Name { get; private set; }

        public Type ServiceType
        {
            get { return typeof( TService ); }
        }

        public ServiceState State { get; protected set; }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public void Initialize()
        {
            if ( Initialized )
                return;

            try
            {
                ServiceInstance = Assimilate.GetInstanceOf<TService>();
            }
            catch ( Exception ex )
            {
                throw ex;
            }
            Initialized = true;
        }

        public void Start()
        {
            try
            {
                ServiceInstance.Start();
            }
            catch ( Exception ex )
            {
                SendFault( ex );
            }
        }

        public void Stop()
        {
            try
            {
                ServiceInstance.Stop();
            }
            catch ( Exception ex )
            {
                SendFault( ex );
            }
        }

        public void Pause()
        {
            try
            {
            }
            catch ( Exception ex )
            {
                SendFault( ex );
            }
        }

        public void Continue()
        {
            try
            {
            }
            catch ( Exception ex )
            {
                SendFault( ex );
            }
        }

        protected void Dispose( bool disposing )
        {
            if ( !disposing )
                return;
            if ( Disposed )
                return;

            ServiceInstance = default(TService);
            PauseAction = null;
            ContinueAction = null;
            Disposed = true;
        }

        ~ServiceController()
        {
            Dispose( false );
        }

        private void SendFault( Exception exception )
        {
            try
            {
            }
            catch ( Exception )
            {
            }
        }

        public ServiceController( TService service )
        {
            ServiceInstance = service;
        }
    }
}