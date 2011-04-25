using System.Runtime.Serialization;

namespace Core.Tests.Serialization.Extensions
{
    [DataContract]
    public class PartiallyMarkedWithContract
        : NoDefaultConstructor
    {
        public PartiallyMarkedWithContract() : base( "test" )
        {
        }
    }
}