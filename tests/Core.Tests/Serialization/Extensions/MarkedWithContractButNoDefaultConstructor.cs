using System.Runtime.Serialization;

namespace Core.Tests.Serialization.Extensions
{
    [DataContract]
    public class MarkedWithContractButNoDefaultConstructor
        : NoDefaultConstructor
    {
        public MarkedWithContractButNoDefaultConstructor( string message ) : base( message )
        {
        }
    }
}