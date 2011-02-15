using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Symbiote.Core;
using Symbiote.Core.Collections;
using Symbiote.Core.Reflection;
using Symbiote.Core.Utility;
using Symbiote.Core.Serialization;

namespace Core.Tests.Serialization 
{
    public interface IHazId
    {
        string Key { get; set; }
    }

    public class IHazIdKeyAccessor : IKeyAccessor<IHazId>
    {
        public string GetId( IHazId actor )
        {
            return actor.Key;
        }

        public void SetId<TKey>( IHazId actor, TKey key )
        {
            actor.Key = key.ToString();
        }
    }

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

    public class Level1b : IHazId
    {
        public string Key { get; set; }
        public string Property1 { get; set; }
        public decimal Property2 { get; set; }
        public DateTime Created { get; set; }
    }

    public class Level2a : IHazId
    {
        public string Key { get; set; }
        public int Amount { get; set; }
        public bool Flag { get; set; }
    }

    public class Level2b : IHazId
    {
        public string Key { get; set; }
        public byte[] Bytes { get; set; }
    }

    public class Level2c : IHazId
    {
        public string Key { get; set; }
        public long BigNumber { get; set; }
    }

    public class Level3 : IHazId
    {
        public string Key { get; set; }
    }

    public class Additive
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string[] _attachments { get; set; }
        public object document { get; set; }
    }

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
