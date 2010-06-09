using System;

namespace Symbiote.Lucene
{
    public class LuceneConfigurator
    {
        protected ILuceneConfiguration configuration { get; set; }

        public LuceneConfigurator UseDefaults()
        {
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