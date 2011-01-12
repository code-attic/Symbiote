using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf.Connection;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl
{
    public class DocumentRepository
        : IDocumentRepository
    {
        public IConnectionProvider ConnectionProvider { get; set; }
        public IBasicCommandFactory CommandFactory { get; set; }

        public Document<T> GetDocument<T>( string bucket, string key, uint minimum )
        {
            var command = CommandFactory.CreateGet( bucket, key, minimum );
            var content = command.Execute();
            return content.ToDocument<T>();
        }

        public void PersistDocument<T>( string bucket, string key, string vectorClock, Document<T> document, uint write, uint dw, bool returnBody )
        {
            var content = new RiakContent( document.Value, document.ContentType, document.Charset, document.ContentEncoding, document.VectorClock, document.LastModified, document.LastModifiedInSeconds );
            var command = CommandFactory.CreatePersist( bucket, key, vectorClock, content, write, dw, returnBody );
        }

        public DocumentRepository( IConnectionProvider connectionProvider, IBasicCommandFactory commandFactory )
        {
            ConnectionProvider = connectionProvider;
            CommandFactory = commandFactory;
        }
    }
}