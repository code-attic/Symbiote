using System.Collections.Generic;

namespace Core.Tests.Serialization
{
    public class Level1a : IHazId
    {
        public string Key { get; set; }
        public IDictionary<string, Level2a> Lookup2a { get; set; }
        public HashSet<Level2b> Set2b { get; set; }
        public Queue<Level2c> Set2c { get; set; }

        public Level1a()
        {
            Lookup2a = new Dictionary<string, Level2a>();
            Set2b = new HashSet<Level2b>();
            Set2c = new Queue<Level2c>();
        }
    }
}