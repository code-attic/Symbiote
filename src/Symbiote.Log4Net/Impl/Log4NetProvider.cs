using System.Linq;
using System.Text;
using Symbiote.Core.Impl.Log.Impl;
using LogManager=log4net.LogManager;

namespace Symbiote.Log4Net.Impl
{
    public class Log4NetProvider : ILogProvider
    {
        public ILogger GetLoggerForType<T>()
        {
            return new Log4NetLogger(LogManager.GetLogger(typeof (T)));
        }
    }
}
