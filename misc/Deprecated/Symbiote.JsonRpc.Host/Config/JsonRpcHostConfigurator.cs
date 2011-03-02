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
using Symbiote.Core;

namespace Symbiote.JsonRpc.Host.Config
{
    public class JsonRpcHostConfigurator
    {
        private IJsonRpcHostConfiguration _configuration;

        public JsonRpcHostConfigurator AddPort(int port)
        {
            _configuration.Ports.Add(port);
            return this;
        }

        public JsonRpcHostConfigurator HostInIIS()
        {
            _configuration.SelfHosted = false;
            return this;
        }

        public IJsonRpcHostConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public JsonRpcHostConfigurator HostService<T>()
            where T : class
        {
            Assimilate.Dependencies(x => x.Scan(s =>
                                                    {
                                                        s.AssembliesFromApplicationBaseDirectory();
                                                        s.TheCallingAssembly();
                                                        s.AddAllTypesOf<T>();
                                                    }));

            _configuration.RegisteredServices.Add(Tuple.Create(typeof(T), Assimilate.Assimilation.DependencyAdapter.GetDefaultTypeFor<T>()));
            return this;
        }

        public JsonRpcHostConfigurator(IJsonRpcHostConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}