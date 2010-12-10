using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl.Command.Set
{
    public class SUnionStoreCommand<TValue> : SEnumInputBoolOutCommand<TValue>
    {
        public SUnionStoreCommand(IEnumerable<string> key, string destKey)
            : base(key, destKey)
        {

        }


        protected override void SetCmd()
        {
            CMD = "SUNIONSTORE";
        }
    }
}
