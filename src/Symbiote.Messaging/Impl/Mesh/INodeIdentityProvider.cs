using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeIdentityProvider
    {
        string Identity { get; }
    }
}
