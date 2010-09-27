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
using Symbiote.Wcf.Client;

namespace Symbiote.Wcf
{
    public static class WcfClientAssimilation
    {
        public static IAssimilate WcfClient(this IAssimilate assimilate, Action<WcfClientConfigurator> configure)
        {
            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IServiceMetadataCache>().Use<InMemoryMetadataCache>();
                                      x.For(typeof (IService<>)).Use(typeof (ServiceClient<>));
                                      x.For(typeof(IServiceClientConfigurationStrategy<>)).Use(typeof(DefaultConfigurationStrategy<>));
                                  });

            var wcfClientConfig = new WcfClientConfigurator();
            configure(wcfClientConfig);

            return assimilate;
        }
    }
}
