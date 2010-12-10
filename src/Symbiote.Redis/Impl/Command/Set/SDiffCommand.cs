using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl.Command.Set
{
    public class SDiffCommand<TValue> : SEnumInputCommand<TValue>
    {

        public SDiffCommand(IEnumerable<string> key)
            : base(key)
        {

        }

        protected override void SetCmd()
        {
            CMD = "SDIFF";
        }
    }
}
