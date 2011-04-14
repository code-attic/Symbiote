using Machine.Specifications;
using Symbiote.Core.DI.Impl;

namespace Core.Tests.DI.Container
{
    public class with_container
    {
        protected static SimpleDependencyRegistry Container { get; set; }
        private Establish context = () => { Container = new SimpleDependencyRegistry(); };
    }
}