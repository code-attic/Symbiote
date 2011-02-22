using Symbiote.Core;
using Symbiote.Core.Log;

namespace Symbiote.Log4Net.Config
{
    public class Log4NetInitializer : IInitializeSymbiote
    {
        public void Initialize()
        {
            if( !LogManager.Initialized )
                LogManager.Initialize();
        }
    }
}