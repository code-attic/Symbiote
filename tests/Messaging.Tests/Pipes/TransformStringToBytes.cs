using System.Text;
using Symbiote.Messaging.Impl.Transform;

namespace Messaging.Tests.Pipes
{
    public class TransformStringToBytes
        : BaseTransform<string, byte[]>
    {
        public override byte[] Transform( string origin )
        {
            return Encoding.UTF8.GetBytes( origin );
        }

        public override string Reverse( byte[] transformed )
        {
            return Encoding.UTF8.GetString( transformed );
        }
    }
}