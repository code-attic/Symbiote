using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl.Command.Set
{
    public class SUnionCommand<TValue> : SEnumInputCommand<TValue>
    {

        public SUnionCommand(IEnumerable<string> key)
            : base(key)
        {

        }

        protected override void SetCmd()
        {
            CMD = "SUNION";
        }
    }
}
