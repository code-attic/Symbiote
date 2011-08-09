using System;
using System.Linq;
using Machine.Specifications;

namespace Core.Tests.Hashing
{
        public class with_substantial_keyset
    {
        public static string[] Keys;
        public static int Total;
        public static ulong[] HashSet1;
        public static ulong[] HashSet2;
        public static long Elasped;

        private Establish context = () => 
        {
            var seed = "ABCDEFGHIJKLMNO";
            Keys = seed
                .ToCharArray()
                .UniquePermutations()
                .Select( x => new String(x.ToArray()) )
                .Distinct()
                .ToArray();
            Total = Keys.Length;
            HashSet1 = new ulong[Total];
            HashSet2 = new ulong[Total];
        };
    }
}
