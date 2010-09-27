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
using Symbiote.JsonRpc.Client.Config;
using Symbiote.JsonRpc.Client.Impl.Rpc;

namespace Symbiote.JsonRpc.Client
{
    public static class JsonRpcClientAssimilation
    {
        public static IAssimilate JsonRpcClient(this IAssimilate assimilate, Action<JsonRpcClientConfigurator> configure)
        {
            var configurator = new JsonRpcClientConfigurator(new JsonRpcClientConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IJsonRpcClientConfiguration>().Use(configurator.GetConfiguration());
                                      x.For(typeof (IRemoteProxy<>)).Use(typeof (RemoteProxy<>));
                                  });

            return assimilate;
        }
    }
}