using Lucene.Net.Store;
using StructureMap;

namespace Symbiote.Lucene
{
    public class DirectoryFactory
    {
        protected ILuceneConfiguration configuration { get; set; }

        public Directory GetDirectory()
        {
            return ObjectFactory.GetInstance(configuration.DirectoryType) as Directory;
        }

        public DirectoryFactory(ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}