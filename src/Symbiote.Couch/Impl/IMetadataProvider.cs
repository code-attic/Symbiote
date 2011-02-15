using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Couch.Impl.Model;

namespace Symbiote.Couch.Impl {
    public interface IMetadataProvider 
    {
        void RemoveKey( object key );
        void AddMetadata( object key, object document );
        DocumentMetadata GetMetadata ( object key );
        void Clear();
    }
}
