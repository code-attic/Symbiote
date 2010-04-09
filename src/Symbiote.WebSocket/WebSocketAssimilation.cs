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
                                      x.For<ISocketServer>().Singleton().Use<WebSocketServer>();
                                      x.For<IWebSocketServerConfiguration>().Use(config.Configuration);
                                      x.For<ICreateWebSockets>().Use<WebSocketFactory>();
                                  });

            return assimilate;
        }
    }
}