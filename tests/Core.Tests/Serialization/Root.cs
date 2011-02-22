using System.Collections.Generic;

namespace Core.Tests.Serialization
{
    public class Root : IHazId
    {
        public string Key { get; set; }
        public IList<Level1a> AList { get; set; }
        public Level1b[] BArray { get; set; }

        public Root()
        {
            AList = new List<Level1a>();
            BArray = new Level1b[5];
        }
    }
}