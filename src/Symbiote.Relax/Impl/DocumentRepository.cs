using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Relax.Impl
{
    public class DocumentRepository<TModel> 
        : BaseDocumentRepository<TModel>
        where TModel : class, ICouchDocument
    {
        public DocumentRepository(ICouchConfiguration configuration, ICouchCommandFactory commandFactory) 
            : base(configuration, commandFactory)
        {
        }

        public DocumentRepository(string configurationName) 
            : base(configurationName)
        {
        }
    }
}
