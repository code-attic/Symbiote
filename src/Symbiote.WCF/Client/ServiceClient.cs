/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Impl.Log;

namespace Symbiote.Wcf.Client
{
    public class ServiceClient<TContract> :
        IService<TContract>,
        IServiceConfiguration
        where TContract : class
    {
        #region Fields

        protected TContract _channelProxy;
        protected ClientShell<TContract> _clientShell;
        protected string _configuration = "";
        protected Binding _binding;
        protected EndpointAddress _endpoint;
        protected string _mexAddress = "";
        protected ILogger<IServiceConfiguration> _logger;
        protected bool _disposingShell = false;
        protected bool _disposingProxy = false;

        public string Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }

        public virtual ClientCredentials ClientCredentials
        {
            get { return ClientShell.ClientCredentials; }
        }

        public Binding Binding
        {
            get { return _binding; }
            set { _binding = value; }
        }

        public EndpointAddress Endpoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        public string MetadataExchangeAddress
        {
            get { return _mexAddress; }
            set
            {
                _mexAddress = value;
                ReadServiceInfoFromMetadataExchange(value);
            }
        }

        #endregion

        public TContract ChannelProxy
        {
            get
            {
                try
                {
                    _channelProxy = _channelProxy ?? ClientShell.ChannelFactory.CreateChannel();
                }
                catch (Exception e)
                {
                    var message = "An exception occurred trying to instantiate the service proxy for {0}"
                        .AsFormat(typeof(TContract).AssemblyQualifiedName);
                    _logger.Log(LogLevel.Error, message as object, e as Exception);
                    throw;
                }
                return _channelProxy;
            }
        }

        protected ClientShell<TContract> ClientShell
        {
            get
            {
                if (_clientShell == null)
                {
                    _clientShell = _endpoint != null ?
                                            new ClientShell<TContract>(_binding, _endpoint) :
                                            new ClientShell<TContract>(_configuration);
                }
                return _clientShell;
            }
        }

        #region Public Methods

        public void Call(Action<TContract> call)
        {
            try
            {
                if (ClientShell.State == CommunicationState.Closed || ClientShell.State == CommunicationState.Closing)
                    ClientShell.Open();

                call(ChannelProxy);
            }
            catch (Exception e)
            {
                var message = string.Format(
                    "While calling service method {0} on {1} an exception occurred", call.Method,
                    typeof(TContract).AssemblyQualifiedName);
                _logger.Log(LogLevel.Error, message as object, e);
                throw;
            }
            finally
            {
                Close();
            }
        }

        public TResult Call<TResult>(Func<TContract, TResult> call)
        {
            TResult result = default(TResult);
            try
            {
                if (ClientShell.State == CommunicationState.Closed || ClientShell.State == CommunicationState.Closing)
                    ClientShell.Open();

                result = call(ChannelProxy);
            }
            catch (Exception e)
            {
                var message = string.Format(
                    "While calling service method {0} on {1} an exception occurred", call.Method,
                    typeof(TContract).AssemblyQualifiedName);
                _logger.Log(LogLevel.Error, message as object, e);
                throw;
            }
            finally
            {
                Close();
            }
            return result;
        }

        public virtual ServiceClient<TContract> AllTimeouts(TimeSpan time)
        {
            _binding.CloseTimeout = time;
            _binding.SendTimeout = time;
            _binding.ReceiveTimeout = time;
            _binding.OpenTimeout = time;
            return this;
        }

        public virtual ServiceClient<TContract> AllTimeouts(TimeSpan closeTimeout, TimeSpan openTimeout, TimeSpan receiveTimeout, TimeSpan sendTimeout)
        {
            _binding.CloseTimeout = closeTimeout;
            _binding.SendTimeout = sendTimeout;
            _binding.ReceiveTimeout = receiveTimeout;
            _binding.OpenTimeout = openTimeout;
            return this;
        }

        #endregion

        #region Constructors

        public ServiceClient(ILogger<IServiceConfiguration> logger)
        {
            _logger = logger;
            _logger.Log(LogLevel.Info, "{0} proxy instantiated", typeof(TContract).AssemblyQualifiedName);
            var configurationStrategy = Assimilate.GetInstanceOf<IServiceClientConfigurationStrategy<TContract>>();
            configurationStrategy.ConfigureServiceClient(this);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _logger.Log(LogLevel.Info, "Disposing service client...");
            DisposeChannelProxy();
            DisposeClientShell();
        }

        public void Close()
        {
            DisposeChannelProxy();
            DisposeClientShell();
        }

        private void DisposeClientShell()
        {
            if (_disposingShell || _clientShell == null)
                return;

            _disposingShell = true;
            switch (_clientShell.State)
            {
                case CommunicationState.Faulted:
                    _clientShell.Abort();
                    break;
                default:
                    try
                    {
                        _clientShell.Close();
                    }
                    catch (Exception e)
                    {
                        _clientShell.Abort();
                    }
                    break;
            }
            if (_clientShell != null)
            {
                (_clientShell as IDisposable).Dispose();
            }
            _clientShell = null;
            _disposingShell = false;
        }

        private void DisposeChannelProxy()
        {
            if (_disposingProxy || _channelProxy == null)
                return;

            _disposingProxy = true;
            (_channelProxy as IDisposable).Dispose();
            _channelProxy = null;
            _disposingProxy = false;
        }

        #endregion

        private void ReadServiceInfoFromMetadataExchange(string mexAddress)
        {
            var cache = Assimilate.GetInstanceOf<IServiceMetadataCache>();
            var serviceEndPoint = cache.GetEndPoint<TContract>(mexAddress);

            _endpoint = serviceEndPoint.Address;
            _binding = serviceEndPoint.Binding;
        }
    }
}