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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;

namespace Symbiote.Wcf.Client
{
    public class ServiceClient<TContract> :
        IService<TContract>,
        IServiceConfiguration
        where TContract : class
    {
        protected bool DisposingProxy;
        protected bool DisposingShell;
        protected ILogger<IServiceConfiguration> Logger;
        protected string MexAddress = "";
        private TContract _channelProxy;
        private ClientShell<TContract> _clientShell;

        public TContract ChannelProxy
        {
            get
            {
                try
                {
                    _channelProxy = _channelProxy ?? ClientShell.ChannelFactory.CreateChannel();
                }
                catch ( Exception e )
                {
                    var message = "An exception occurred trying to instantiate the service proxy for {0}"
                        .AsFormat( typeof( TContract ).AssemblyQualifiedName );
                    Logger.Log( LogLevel.Error, message as object, e );
                    throw;
                }
                return _channelProxy;
            }
        }

        protected ClientShell<TContract> ClientShell
        {
            get
            {
                return _clientShell ?? (_clientShell = Endpoint != null
                                                           ? new ClientShell<TContract>( Binding, Endpoint )
                                                           : new ClientShell<TContract>( Configuration ));
            }
        }

        public virtual ClientCredentials ClientCredentials
        {
            get { return ClientShell.ClientCredentials; }
        }

        public void Call( Action<TContract> call )
        {
            try
            {
                if ( ClientShell.State == CommunicationState.Closed || ClientShell.State == CommunicationState.Closing )
                    ClientShell.Open();

                call( ChannelProxy );
            }
            catch ( Exception e )
            {
                var message = string.Format(
                    "While calling service method {0} on {1} an exception occurred", call.Method,
                    typeof( TContract ).AssemblyQualifiedName );
                Logger.Log( LogLevel.Error, message as object, e );
                throw;
            }
            finally
            {
                Close();
            }
        }

        public TResult Call<TResult>( Func<TContract, TResult> call )
        {
            TResult result;
            try
            {
                if ( ClientShell.State == CommunicationState.Closed || ClientShell.State == CommunicationState.Closing )
                    ClientShell.Open();

                result = call( ChannelProxy );
            }
            catch ( Exception e )
            {
                var message = string.Format(
                    "While calling service method {0} on {1} an exception occurred", call.Method,
                    typeof( TContract ).AssemblyQualifiedName );
                Logger.Log( LogLevel.Error, message as object, e );
                throw;
            }
            finally
            {
                Close();
            }
            return result;
        }

        public void Dispose()
        {
            Logger.Log( LogLevel.Info, "Disposing service client..." );
            DisposeChannelProxy();
            DisposeClientShell();
        }

        public void Close()
        {
            DisposeChannelProxy();
            DisposeClientShell();
        }

        public string Configuration { get; set; }

        public Binding Binding { get; set; }

        public EndpointAddress Endpoint { get; set; }

        public string MetadataExchangeAddress
        {
            get { return MexAddress; }
            set
            {
                MexAddress = value;
                ReadServiceInfoFromMetadataExchange( value );
            }
        }

        public virtual ServiceClient<TContract> AllTimeouts( TimeSpan time )
        {
            Binding.CloseTimeout = time;
            Binding.SendTimeout = time;
            Binding.ReceiveTimeout = time;
            Binding.OpenTimeout = time;
            return this;
        }

        public virtual ServiceClient<TContract> AllTimeouts( TimeSpan closeTimeout, TimeSpan openTimeout,
                                                             TimeSpan receiveTimeout, TimeSpan sendTimeout )
        {
            Binding.CloseTimeout = closeTimeout;
            Binding.SendTimeout = sendTimeout;
            Binding.ReceiveTimeout = receiveTimeout;
            Binding.OpenTimeout = openTimeout;
            return this;
        }

        private void DisposeClientShell()
        {
            if ( DisposingShell || _clientShell == null )
                return;

            DisposingShell = true;
            switch( _clientShell.State )
            {
                case CommunicationState.Faulted:
                    _clientShell.Abort();
                    break;
                default:
                    try
                    {
                        _clientShell.Close();
                    }
                    catch ( Exception )
                    {
                        _clientShell.Abort();
                    }
                    break;
            }
            if ( _clientShell != null )
            {
                (_clientShell as IDisposable).Dispose();
            }
            _clientShell = null;
            DisposingShell = false;
        }

        private void DisposeChannelProxy()
        {
            if ( DisposingProxy || _channelProxy == null )
                return;

            DisposingProxy = true;
            (_channelProxy as IDisposable).Dispose();
            _channelProxy = null;
            DisposingProxy = false;
        }

        private void ReadServiceInfoFromMetadataExchange( string mexAddress )
        {
            var cache = Assimilate.GetInstanceOf<IServiceMetadataCache>();
            var serviceEndPoint = cache.GetEndPoint<TContract>( mexAddress );
            Configuration = "";
            Endpoint = serviceEndPoint.Address;
            Binding = serviceEndPoint.Binding;
        }

        public ServiceClient( ILogger<IServiceConfiguration> logger )
        {
            Logger = logger;
            Logger.Log( LogLevel.Info, "{0} proxy instantiated", typeof( TContract ).AssemblyQualifiedName );
            var configurationStrategy = Assimilate.GetInstanceOf<IServiceClientConfigurationStrategy<TContract>>();
            configurationStrategy.ConfigureServiceClient( this );
        }
    }
}