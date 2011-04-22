using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Mesh;

namespace Messaging.Tests.Configuration
{
    public class when_checking_handlers : with_assimilation
    {
        private static IHandle<NodeHealth> nodeHealthHandler;
        private static IHandle<NodeUp> nodeUpHandler;
        private static IHandle<NodeDown> nodeDownHandler;

        private Because of = () =>
            {
                nodeHealthHandler = Assimilate.GetInstanceOf<IHandle<NodeHealth>>();
                nodeUpHandler = Assimilate.GetInstanceOf<IHandle<NodeUp>>();
                nodeDownHandler = Assimilate.GetInstanceOf<IHandle<NodeDown>>();
            };

        private It should_have_nodeHealthHandler = () => nodeHealthHandler.ShouldNotBeNull();
        private It should_have_nodeUpHandler = () => nodeUpHandler.ShouldNotBeNull();
        private It should_have_nodeDownHandler = () => nodeDownHandler.ShouldNotBeNull();
    }
}
