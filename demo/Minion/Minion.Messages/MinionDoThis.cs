using System.Runtime.Serialization;

namespace Minion.Messages
{
    [DataContract]
    public class MinionDoThis
    {
        [DataMember(Order = 1)]
        public string Text { get; set; }
    }
}
