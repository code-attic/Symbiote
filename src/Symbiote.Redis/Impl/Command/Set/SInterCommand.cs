using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl.Command.Set
{
    public class SInterCommand<TValue>: SEnumInputCommand<TValue>
    {

        public SInterCommand(IEnumerable<string> key)
            : base(key)
        {
            
        }

        protected override void SetCmd()
        {
            CMD = "SINTER";
        }
    }
}
