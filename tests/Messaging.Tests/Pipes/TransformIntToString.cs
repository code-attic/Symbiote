using Symbiote.Messaging.Impl.Transform;

namespace Messaging.Tests.Pipes
{
    public class TransformIntToString
        : BaseTransform<int, string>
    {
        public override string Transform( int origin )
        {
            return origin.ToString();
        }

        public override int Reverse( string transformed )
        {
            return int.Parse( transformed );
        }
    }
}