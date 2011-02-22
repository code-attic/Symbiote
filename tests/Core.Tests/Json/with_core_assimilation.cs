using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests.Json
{
    public abstract class with_core_assimilation
    {
        private Establish context = () => Assimilate.Initialize();
    }
}