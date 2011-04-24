using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate.Initialize();
                                        };
    }
}