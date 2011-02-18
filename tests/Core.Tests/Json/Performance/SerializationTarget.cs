using System;

namespace Core.Tests.Json.Performance
{
    [Serializable]
    public class SerializationTarget : IGotMessageForYou
        
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}