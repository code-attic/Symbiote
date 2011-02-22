using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests.Actor.KeyAccess
{
    public class with_key_accessor 
        : with_assimilation
    {
        public static IKeyAccessor KeyAccessor { get; set; }
        private Establish context = () =>
                                        {
                                            KeyAccessor = Assimilate.GetInstanceOf<IKeyAccessor>();
                                        };
    }
}