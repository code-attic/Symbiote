using System;

namespace Core.Tests.Serialization
{
    public class Level1b : IHazId
    {
        public string Key { get; set; }
        public string Property1 { get; set; }
        public decimal Property2 { get; set; }
        public DateTime Created { get; set; }
    }
}