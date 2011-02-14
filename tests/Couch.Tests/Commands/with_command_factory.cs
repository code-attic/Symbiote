using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Couch.Impl.Commands;

namespace Couch.Tests.Commands
{
    public abstract class with_command_factory : with_mock_http_action
    {
        protected static CouchCommandFactory factory;

        private Establish context = () =>
                                        {
                                            factory = Assimilate.GetInstanceOf<CouchCommandFactory>();
                                        };
    }
}