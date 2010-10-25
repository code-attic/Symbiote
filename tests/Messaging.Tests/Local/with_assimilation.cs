using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Log.Impl;
using Symbiote.Messaging;
using Symbiote.StructureMap;

namespace Messaging.Tests.Local
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Messaging()
                                                .UseTestLogAdapter();
                                        };    
    }
}