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
using Symbiote.WebSocket.Impl;

namespace Symbiote.WebSocket
{
    public static class WebSocketAssimilation
    {
        public static IAssimilate WebSocketServer(this IAssimilate assimilate, Action<WebSocketConfigurator> configurator)
        {
            var config = new WebSocketConfigurator();
            configurator(config);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IShakeHands>().Use<DefaultHandShake>();
                                      x.For<IHandlePolicyRequests>().Use<DefaultPolicyRequestHandler>();
                                      x.For<ISocketServer>().Use<WebSocketServer>().AsSingleton();
                                      x.For<IWebSocketServerConfiguration>().Use(config.Configuration);
                                      x.For<ICreateWebSockets>().Use<WebSocketFactory>();
                                  });

            return assimilate;
        }
    }
}