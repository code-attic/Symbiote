using Symbiote.Core;
using Symbiote.Core.Log;

namespace Symbiote.Log4Net.Config
{
    public class Log4NetInitializer : IInitialize
    {
        public void Initialize()
        {
            var logManager = Assimilate.GetInstanceOf<ILogManager>();
            logManager.Initialize();
        }
    }
}