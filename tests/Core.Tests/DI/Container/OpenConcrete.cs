namespace Core.Tests.DI.Container
{
    public class OpenConcrete<T>
    {
        public IMessageProvider Provider { get; set; }

        public OpenConcrete( IMessageProvider provider )
        {
            Provider = provider;
        }
    }
}