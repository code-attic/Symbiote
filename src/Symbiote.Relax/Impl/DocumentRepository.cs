using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Relax.Impl
{
    public class DocumentRepository : BaseDocumentRepository<Guid, string>, IDocumentRepository, IDocumentRepository<Guid,String>
    {
        public DocumentRepository(ICouchConfiguration configuration) : base(configuration)
        {
        }

        public DocumentRepository(string configurationName) : base(configurationName)
        {
            
        }
    }

    public class DocumentRepository<TModel> 
        : BaseDocumentRepository<TModel, Guid, string>, IDocumentRepository<TModel>
        where TModel : class, ICouchDocument<Guid, string>
    {
        public DocumentRepository(IDocumentRepository<Guid, string> repository) : base(repository)
        {
        }

        public DocumentRepository(string configurationName) : base(configurationName)
        {
            
        }
    }

    public class DocumentRepository<TModel, TKey, TRev> 
        : BaseDocumentRepository<TModel, TKey, TRev>, IDocumentRepository<TModel, TKey, TRev>
        where TModel : class, ICouchDocument<TKey, TRev>
    {
        public DocumentRepository(IDocumentRepository<TKey, TRev> repository)
            : base(repository)
        {
        }

        public DocumentRepository(string configurationName) : base(configurationName)
        {

        }
    }
}
