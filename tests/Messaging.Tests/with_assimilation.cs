using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.StructureMapAdapter;

namespace Messaging.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Initialize()
                                                .UseTestLogAdapter();
                                        };    
    }
}