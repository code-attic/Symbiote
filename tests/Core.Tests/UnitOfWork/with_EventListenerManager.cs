using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.UnitOfWork
{
    public class with_EventListenerManager : with_assimilation
    {
        public static IEventListenerManager Manager;

        private Establish context = () =>
                                        {
                                            Manager = Assimilate.GetInstanceOf<IEventListenerManager>();
                                        };
    }
}
