namespace Core.Tests.DI.Container
{
    public class PlainConcrete
    {
        public IMessageProvider Provider { get; set; }

        public PlainConcrete( IMessageProvider provider )
        {
            Provider = provider;
        }
    }
}