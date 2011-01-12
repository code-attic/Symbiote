using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis.Impl.Connection
{
    public interface IConnectionProvider
    {
         IConnectionHandle Acquire();
    }
}
