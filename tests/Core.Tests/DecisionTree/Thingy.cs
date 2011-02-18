using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Tests.Serialization;

namespace Core.Tests.DecisionTree 
{
    public class Thingy : IHazId
    {
        public string Key { get; set; }

        public bool Flag1 { get; set; }
        public bool Flag2 { get; set; }
        public bool Flag3 { get; set; }

        public Thingy()
        {
        }

        public Thingy( string key, bool flag1, bool flag2, bool flag3 )
        {
            Key = key;
            Flag1 = flag1;
            Flag2 = flag2;
            Flag3 = flag3;
        }
    }
}
