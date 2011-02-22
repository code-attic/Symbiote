using System;
using System.Collections.Generic;

namespace Core.Tests.Serialization
{
    public class with_ridiculous_model_instance
    {
        public static Root root 
            = new Root()
                  {
                      Key = "root",
                      AList = GetAList(),
                      BArray = GetBArray()
                  };

        public static List<Level1a> GetAList()
        {
            return new List<Level1a>()
                       {
                           new Level1a() { Key = "Lvl1a-1", Lookup2a = GetLookup2a(1), Set2b = GetSet2b(1), Set2c = GetSet2c(1) },
                           new Level1a() { Key = "Lvl1a-2", Lookup2a = GetLookup2a(2), Set2b = GetSet2b(2), Set2c = GetSet2c(2) },
                           new Level1a() { Key = "Lvl1a-3", Lookup2a = GetLookup2a(3), Set2b = GetSet2b(3), Set2c = GetSet2c(3) },
                       };
        }

        public static Level1b[] GetBArray()
        {
            return new Level1b[]
                       {
                           new Level1b() { Key = "Lvl1b-1", Created = DateTime.UtcNow, Property1 = "Hi from lvl1 b, 1", Property2 = .001m },
                           new Level1b() { Key = "Lvl1b-2", Created = DateTime.UtcNow, Property1 = "Hi from lvl1 b, 2", Property2 = .0012m },
                           new Level1b() { Key = "Lvl1b-3", Created = DateTime.UtcNow, Property1 = "Hi from lvl1 b, 3", Property2 = .00123m },
                           new Level1b() { Key = "Lvl1b-4", Created = DateTime.UtcNow, Property1 = "Hi from lvl1 b, 4", Property2 = .001234m },
                       };
        }

        public static Dictionary<string, Level2a> GetLookup2a( int number )
        {
            return new Dictionary<string, Level2a>()
                       {
                           { "one", new Level2a() { Amount = 1, Key = "one", Flag = true } },
                           { "two", new Level2a() { Amount = 2, Key = "two", Flag = true } },
                           { "three", new Level2a() { Amount = 3, Key = "three", Flag = true } },
                       };
        }

        public static HashSet<Level2b> GetSet2b( int number )
        {
            return new HashSet<Level2b>();
        }

        public static Queue<Level2c> GetSet2c( int number )
        {
            return new Queue<Level2c>();
        }
    }
}