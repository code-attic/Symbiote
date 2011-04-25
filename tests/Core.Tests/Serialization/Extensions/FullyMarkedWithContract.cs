using System.Runtime.Serialization;

namespace Core.Tests.Serialization.Extensions
{
    [DataContract]
    public class FullyMarkedWithContract
    {
        [DataMember()]
        public string Message { get; set; }

        public FullyMarkedWithContract()
        {
        }
    }
}