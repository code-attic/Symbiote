using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl.Command.Set
{
    public class SInterStoreCommand<TValue> : SEnumInputBoolOutCommand<TValue>
    {
        public SInterStoreCommand(IEnumerable<string> key, string destKey)
            : base(key, destKey)
        {
            
        }


        protected override void SetCmd()
        {
            CMD = "SINTERSTORE";
        }
    }
}
