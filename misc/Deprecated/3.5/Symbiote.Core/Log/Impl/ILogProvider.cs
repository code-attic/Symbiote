using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Log.Impl
{
    public interface ILogProvider
    {
        ILogger GetLoggerForType<T>();
    }
}
