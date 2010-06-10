using Lucene.Net.Store;
using Symbiote.Lucene.Config;

namespace Symbiote.Lucene.Impl
{
    public class DefaultDirectoryFactory : IDirectoryFactory
    {
        protected ILuceneConfiguration configuration;

        public Directory CreateDirectoryFor(string indexName)
        {
            string indexDirectory = null;
            configuration.DirectoryPaths.TryGetValue(indexName, out indexDirectory);
            indexDirectory = indexDirectory ?? System.IO.Path.Combine(configuration.DefaultIndexPath, indexName);
            var directoryInfo = System.IO.Directory.CreateDirectory(indexDirectory);
            var qualifiedDirectoryPath = System.IO.Path.GetFullPath(directoryInfo.ToString());

            var simpleFsDirectory = new SimpleFSDirectory(
                directoryInfo,
                new SimpleFSLockFactory());

            simpleFsDirectory.EnsureOpen();

            return simpleFsDirectory;
        }

        public DefaultDirectoryFactory (ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}