using System.Runtime.Serialization;

namespace Minion.Messages
{
    [DataContract]
    public class MinionUp
    {
        [DataMember(Order = 1)]
        public string Text { get; set; }
    }
}
