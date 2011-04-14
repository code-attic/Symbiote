using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Redis;

namespace Redis.Tests
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
