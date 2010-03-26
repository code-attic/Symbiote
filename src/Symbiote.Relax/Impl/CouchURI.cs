using System.Text;

namespace Symbiote.Relax.Impl
{
    public class CouchURI
    {
        private StringBuilder _builder = new StringBuilder();
        private bool _hasArguments = false;

        public static CouchURI Build(string prefix, string server, int port, string database)
        {
            return new CouchURI(prefix, server, port, database);
        }

        public CouchURI ListAll() 
        {
            _builder.Append("/_all_docs");
            return this;
        }

        public CouchURI Changes(Feed feed, int since)
        {
            _builder.Append("/_changes");

            if (feed != Feed.Simple)
            {
                _builder.AppendFormat("?feed={0}",
                                      feed == Feed.Continuous
                                          ? "continuous"
                                          : "longpoll");
            }
            _builder.AppendFormat("&since={0}", since);
            _hasArguments = true;
            return this;
        }

        public CouchURI Design(string designDocumentName)
        {
            _builder.AppendFormat("/_design/{0}", designDocumentName);
            return this;
        }

        public CouchURI View(string viewName)
        {
            _builder.AppendFormat("/_view/{0}", viewName);
            return this;
        }

        public CouchURI List(string listName)
        {
            _builder.AppendFormat("/_list/{0}", listName);
            return this;
        }

        public CouchURI Descending()
        {
            _builder.AppendFormat("{0}descending=true",
                                  _hasArguments ? "&" : "?");

            if (!_hasArguments)
                _hasArguments = true;

            return this;
        }

        public CouchURI Limit(int limit)
        {
            _builder.AppendFormat("{0}limit={1}",
                                  _hasArguments ? "&" : "?", limit);

            if (!_hasArguments)
                _hasArguments = true;

            return this;
        }

        public CouchURI Skip(int number)
        {
            _builder.AppendFormat("{0}skip={1}",
                                  _hasArguments ? "&" : "?", number);

            if (!_hasArguments)
                _hasArguments = true;

            return this;
        }

        public CouchURI Format(string format)
        {
            _builder.AppendFormat("{0}format={1}",
                                  _hasArguments ? "&" : "?", format);

            if (!_hasArguments)
                _hasArguments = true;

            return this;
        }

        public CouchURI KeyAndRev<TKey, TRev>(TKey key, TRev rev)
        {
            if (!_hasArguments)
                _hasArguments = true;

            _builder.AppendFormat("/key={0}?rev={1}",
                                  key,
                                  rev);
            return this;
        }

        public CouchURI Key<TKey>(TKey key)
        {
            if (!_hasArguments)
                _hasArguments = true;

            _builder.AppendFormat("/{0}", key);

            return this;
        }

        public CouchURI ByRange<TKey>(TKey start, TKey end)
        {
            _builder.AppendFormat("{0}startkey={1}&endkey={2}",
                                  _hasArguments ? "&" : "?",
                                  start,
                                  end);

            if (!_hasArguments)
                _hasArguments = true;

            return this;
        }

        public CouchURI IncludeDocuments()
        {
            _builder.AppendFormat("{0}include_docs=true", _hasArguments
                                                              ? "&"
                                                              : "?");

            if (!_hasArguments)
                _hasArguments = true;

            return this;
        }

        public CouchURI BulkInsert()
        {
            _builder.Append("/_bulk_docs");
            return this;
        }

        public CouchURI(string prefix, string server, int port, string database)
        {
            _builder
                .AppendFormat(@"{0}://{1}:{2}/{3}", prefix, server, port, database);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}