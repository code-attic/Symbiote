using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Symbiote.Core;
using Symbiote.Core.Collections;
using Symbiote.Core.Reflection;
using Symbiote.Core.Utility;
using Symbiote.Core.Serialization;

namespace Core.Tests.Serialization 
{
    public class IHazIdKeyAccessor : IKeyAccessor<IHazId>
    {
        public string GetId( IHazId actor )
        {
            return actor.Key;
        }

        public void SetId<TKey>( IHazId actor, TKey key )
        {
            actor.Key = key.ToString();
        }
    }
}
