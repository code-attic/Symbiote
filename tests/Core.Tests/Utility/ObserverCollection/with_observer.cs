using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility.ObserverCollection
{
    public class with_observer
    {
        protected static ObserverCollection<string> Observers;
        protected static TestObserver Observer;

        private Establish context = () => 
                                        { 
                                            Observers = new ObserverCollection<string>();
                                            Observer = new TestObserver();
                                        };
    }
}