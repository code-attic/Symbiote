namespace Symbiote.Lucene.Config
{
    public class LuceneConfigurator
    {
        protected ILuceneConfiguration configuration { get; set; }

        public LuceneConfigurator RootIndexPath(string rootPath)
        {
            configuration.IndexPath = rootPath;
            return this;
        }

        public LuceneConfigurator LimitMemoryBuffer(int megabytes)
        {
            configuration.MemoryBufferLimit = megabytes;
            return this;
        }

        public LuceneConfigurator WriterLockTimeout(long miliseconds)
        {
            configuration.WriterLockTimeout = miliseconds;
            return this;
        }

        public LuceneConfigurator()
        {
            configuration = new LuceneConfiguration();
        }

        public ILuceneConfiguration GetConfiguration()
        {
            return configuration;
        }
    }
}