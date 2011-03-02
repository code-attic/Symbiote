using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Symbiote.Actor.Impl.Memento
{
    [Serializable]
    [DataContract]
    public class PassthroughMemento<TActor> :
        IMemento<TActor>
    {
        [DataMember(Order = 1)]
        public TActor Actor { get; set; }

        public void Capture( TActor instance )
        {
            Actor = instance;
        }

        public void Reset( TActor instance )
        {
            // can't do this here
        }

        public TActor Retrieve()
        {
            return Actor;
        }

        public PassthroughMemento() {}
    }
}
