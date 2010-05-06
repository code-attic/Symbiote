namespace Symbiote.Core.Log.Impl
{
    public class NullLogProvider : ILogProvider
    {
        public ILogger GetLoggerForType<T>()
        {
            return new NullLogger();
        }
    }
}